// See https://aka.ms/new-console-template for more information
using MapperTest;

public interface IMapper
{
  TDest Map<TSource, TDest>(TSource source) where TDest : new();
  TDest Map<TDest>(object source) where TDest : new();
  object Map(Type destType, object source);
}

public class Mapper : IMapper
{
  private readonly MapperConfiguration _configuration;

  public Mapper(MapperConfiguration configuration)
  {
    _configuration = configuration;
  }

  public TDest Map<TSource, TDest>(TSource source) where TDest : new() => Map<TDest>(source);

  public TDest Map<TDest>(object source) where TDest : new() => (TDest)Map(typeof(TDest), source);

  public object Map(Type destType, object source)
  {
    var sourceProps = source.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
    var destProps = destType.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

    var result = Activator.CreateInstance(destType);

    var config = _configuration.TypeMappings.FirstOrDefault(x => x.SourceType == source.GetType() && x.DestType == destType);

    foreach (var destProp in destProps)
    {
      bool hasConfig = false;
      object? value = null;
      if (config != null)
      {
        var memberMapping = config.MemberMappings.FirstOrDefault(x => x.MemberName == destProp.Name);

        if(memberMapping != null)
        {
          hasConfig = true;
          if (memberMapping.Action is IgnoreMappingAction)
            continue;
          else if(memberMapping.Action is MapAction mapAction)
          {
            value = mapAction.Action.DynamicInvoke(source);
          }
        }
      }
      if (!hasConfig)
      {
        var sourceProp = sourceProps.FirstOrDefault(x => x.Name == destProp.Name);
        if (sourceProp != null && sourceProp.CanRead && destProp.CanWrite)
        {
          value = sourceProp.GetValue(source);
        }
      }     

      if (value == null || !value.GetType().IsAssignableTo(destProp.PropertyType))
        value = Map(destProp.PropertyType, value);

      destProp.SetValue(result, value);
    }

    return result;
  }
}
