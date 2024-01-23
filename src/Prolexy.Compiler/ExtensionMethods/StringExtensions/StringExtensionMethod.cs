using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.StringExtensions;

public abstract record StringExtensionMethod(string Name, IEnumerable<Parameter> Parameters, IType ReturnType) :
    Method(Name, PrimitiveType.String, Parameters, ReturnType)
{
    public override bool Accept(object value, bool implicitAccessMethod)
    {
        return value is JToken { Type: JTokenType.String } or String;
    }
}