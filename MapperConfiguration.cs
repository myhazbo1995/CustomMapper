using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapperTest
{
  public interface IMappingAction { };
  public record IgnoreMappingAction : IMappingAction { };
  public record MapAction(Delegate Action) : IMappingAction { };

  public record MemberMappingConfiguration(string MemberName, IMappingAction Action);
  public record MapperConfiguration(IEnumerable<TypeMappingConfiguration> TypeMappings);
  public record TypeMappingConfiguration(Type SourceType, Type DestType, IEnumerable<MemberMappingConfiguration> MemberMappings);
}
