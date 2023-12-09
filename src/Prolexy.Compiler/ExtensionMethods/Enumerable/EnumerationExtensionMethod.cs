using System.Collections;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.Enumerable;

public abstract record EnumerationExtensionMethod(string Name, IType ReturnType) : Method(Name, ReturnType)
{
    public override bool Accept(object value)
    {
        return value is JArray or IEnumerable;
    }
}