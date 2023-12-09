using Newtonsoft.Json.Linq;

namespace Prolexy.Compiler.Models;

public class EnumerableType : IType
{
    public IType? ElementType { get; }

    public EnumerableType(IType? elementType)
    {
        ElementType = elementType;
    }
    public IType? GetSubType(string name)
    {
        return null;
    }

    public bool Accept(object value)
    {
        return value is JArray;
    }
}