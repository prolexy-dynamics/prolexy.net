using Newtonsoft.Json.Linq;
using Prolexy.Compiler.SchemaGenerators;

namespace Prolexy.Compiler.Models;

public class PrimitiveType : IType
{
    private readonly object[] _acceptedTypes;
    public string Name { get; }

    public PrimitiveType(string name, object[] acceptedTypes)
    {
        _acceptedTypes = acceptedTypes;
        Name = name;
    }

    public static readonly PrimitiveType Void = new("void",
        new object[] { JTokenType.Float, JTokenType.Integer });
    
    public static readonly PrimitiveType Number = new("number",
        new object[] { JTokenType.Float, JTokenType.Integer });

    public static readonly PrimitiveType String = new("string",
        new object[] { JTokenType.String });

    public static readonly PrimitiveType Datetime = new("datetime",
        new object[] { JTokenType.Date });

    public static readonly PrimitiveType Boolean = new("boolean",
        new object[] { JTokenType.Boolean });

    public static readonly PrimitiveType Enum = new("enum",
        Array.Empty<object>());

    public IType? GetPropertyType(string name)
    {
        return null;
    }

    public bool Accept(object value)
    {
        return value is JToken jValue
            ? _acceptedTypes.Cast<JTokenType>().Any(a => a == jValue.Type)
            : _acceptedTypes.Cast<Type>().Any(a => a.IsInstanceOfType(value));
    }

    public ITypeData GetTypeData() => new PrimitiveTypeData(Name);
    public ITypeData GetTypeData(SchemaGenerator generator) => new PrimitiveTypeData(Name);
}