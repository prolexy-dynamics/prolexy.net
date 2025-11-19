using Newtonsoft.Json.Linq;
using Prolexy.Compiler.SchemaGenerators;

namespace Prolexy.Compiler.Models;

public record Dynamic : IType
{
    private Dynamic()
    {
        
    }
    public static readonly Dynamic Instance = new Dynamic();
    public string Name { get; } = "Dynamic";
    public IType? GetPropertyType(string name)
    {
        return this;
    }

    public ITypeData GetTypeData(SchemaGenerator generator)
    {
        return new DynamicTypeData();
    }

    public Type? ToClrType()
    {
        return typeof(JObject);
    }
}