using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MapperTest
{
  public interface IMemberMappingConfigurationBuilder
  {
    MemberMappingConfiguration Build();
  };

  public class MemberMappingConfigurationBuilder<TSource, TMember> : IMemberMappingConfigurationBuilder
  {
    private bool _ignore = false;
    private Func<TSource, TMember> _mapAction;
    private readonly string _memberName;

    public MemberMappingConfigurationBuilder(string memberName)
    {
      _memberName = memberName;
    }
    public void Ignore()
    {
      _ignore = true;
    }
    public void Map(Func<TSource, TMember> mapAction)
    {
      _mapAction = mapAction;
    }

    public MemberMappingConfiguration Build()
    {
      return new MemberMappingConfiguration(_memberName, _ignore ? new IgnoreMappingAction() : new MapAction(_mapAction));
    }
  }

  public interface ITypeMappingBuilder
  {
    TypeMappingConfiguration Build();
  }

  public class TypeMappingBuilder<TSource, TDest> : ITypeMappingBuilder
  {
    private readonly List<IMemberMappingConfigurationBuilder> _builders = new();
    public TypeMappingBuilder<TSource, TDest> ForMember<TProp>(string destMemberName, Action<MemberMappingConfigurationBuilder<TSource, TProp>> memberOptions)
    {
      var builder = new MemberMappingConfigurationBuilder<TSource, TProp>(destMemberName);
      _builders.Add(builder);

      memberOptions(builder);
      return this;
    }

    public TypeMappingBuilder<TSource, TDest> ForMember<TProp>(Expression<Func<TDest, TProp>> destMember, Action<MemberMappingConfigurationBuilder<TSource, TProp>> memberOptions)
    {
      var memberName = ((MemberExpression)destMember.Body).Member.Name;
      return ForMember(memberName, memberOptions);
    }

    public TypeMappingConfiguration Build()
    {
      return new TypeMappingConfiguration(typeof(TSource), typeof(TDest), _builders.Select(x => x.Build()));
    }
  }

  public class MapperConfigurationBuilder
  {
    private List<ITypeMappingBuilder> _builders = new();

    public TypeMappingBuilder<TSource, TDest> CreateMap<TSource, TDest>()
    {
      var builder = new TypeMappingBuilder<TSource, TDest>();
      _builders.Add(builder);

      return builder;
    }

    public MapperConfiguration Build()
    {
      return new MapperConfiguration(_builders.Select(x => x.Build()));
    }
  }
}
