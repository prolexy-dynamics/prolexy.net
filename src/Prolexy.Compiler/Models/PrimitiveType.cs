using Newtonsoft.Json.Linq;

namespace Prolexy.Compiler.Models;

public class PrimitiveType : IType
{
    private readonly object[] _acceptedTypes;
    public string TypeName { get; }

    public PrimitiveType(string typeName, object[] acceptedTypes)
    {
        _acceptedTypes = acceptedTypes;
        TypeName = typeName;
    }

    public static readonly PrimitiveType Number = new PrimitiveType("number",
        new object[] { JTokenType.Float, JTokenType.Integer });

    public static readonly PrimitiveType String = new PrimitiveType("string",
        new object[] { JTokenType.String });

    public static readonly PrimitiveType Datetime = new PrimitiveType("datetime",
        new object[] { JTokenType.Date });

    public static readonly PrimitiveType Boolean = new PrimitiveType("boolean",
        new object[] { JTokenType.Boolean });

    public static readonly PrimitiveType Enum = new PrimitiveType("enum",
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
}