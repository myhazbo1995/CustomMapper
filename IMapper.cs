// See https://aka.ms/new-console-template for more information

public interface IMapper {
  TDest Map<TSource, TDest>(TSource source);
  TDest Map<TDest>(object source);
  object Map(Type destType, object source);
}
