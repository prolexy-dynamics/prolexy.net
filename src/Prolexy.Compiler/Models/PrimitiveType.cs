using Newtonsoft.Json.Linq;

namespace Prolexy.Compiler.Models;

public class PrimitiveType : IType
{
    private readonly JTokenType[] _acceptedTypes;
    public string TypeName { get; }

    public PrimitiveType(string typeName, JTokenType[] acceptedTypes)
    {
        _acceptedTypes = acceptedTypes;
        TypeName = typeName;
    }

    public static readonly PrimitiveType Number = new PrimitiveType("number",
        new[] { JTokenType.Float, JTokenType.Integer });

    public static readonly PrimitiveType String = new PrimitiveType("string",
        new[] { JTokenType.String });
    public static readonly PrimitiveType Datetime = new PrimitiveType("datetime",
        new[] { JTokenType.Date });
    public static readonly PrimitiveType Boolean = new PrimitiveType("boolean",
        new[] { JTokenType.Boolean });
    public static readonly PrimitiveType Enum = new PrimitiveType("enum",
        Array.Empty<JTokenType>());

    public IType? GetSubType(string name)
    {
        return null;
    }

    public bool Accept(JToken? value)
    {
        return _acceptedTypes.Any(a => a == value?.Type);
    }
}