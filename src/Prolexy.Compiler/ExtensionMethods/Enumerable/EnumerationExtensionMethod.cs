using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.Enumerable;

public abstract record EnumerationExtensionMethod(string Name, IType ReturnType) : Method(Name, ReturnType)
{
    public override bool Accept(JToken? value)
    {
        return value is { Type: JTokenType.Array };
    }
}