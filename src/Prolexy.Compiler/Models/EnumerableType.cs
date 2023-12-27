using Newtonsoft.Json.Linq;
using Prolexy.Compiler.SchemaGenerators;

namespace Prolexy.Compiler.Models;

public class EnumerableType : IType
{
    public IType? ElementType { get; }

    public EnumerableType(IType? elementType)
    {
        ElementType = elementType;
    }

    public string Name => $"Enumerable<{ElementType!.Name}>";

    public IType? GetPropertyType(string name)
    {
        return null;
    }

    public ITypeData GetTypeData(SchemaGenerator generator)
    {
        return new EnumerableTypeData
        {
            Name = Name
        };
    }

    public bool Accept(object value)
    {
        return value is JArray;
    }
}