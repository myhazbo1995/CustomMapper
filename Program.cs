// See https://aka.ms/new-console-template for more information
using MapperTest;
using System.Linq.Expressions;


var builder = new MapperConfigurationBuilder();
builder.CreateMap<MapTest, MapTestDto>()
  .ForMember(x => x.FullName, (y) => y.Map(z => "xx: "+z.Name));

var mapper = new Mapper(builder.Build(), Array.Empty<ICustomTypeMapper>());

var test = new MapTest() { Name = "1234"};
var result = mapper.Map<MapTest, MapTestDto>(test);

Console.WriteLine(result.FullName);


class MapTest
{
  public string Name { get; set; }
}

class MapTestDto
{
  public string FullName { get; set; }
}





