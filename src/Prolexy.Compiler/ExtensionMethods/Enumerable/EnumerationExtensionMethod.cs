using System.Collections;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.Enumerable;

public abstract record EnumerationExtensionMethod(string Name, IEnumerable<Parameter> Parameters, IType ReturnType) : 
    Method(Name, new EnumerableType(new GenericType("TElement")),Parameters, ReturnType)
{
    public override bool Accept(object value, bool implicitAccessMethod)
    {
        return value is JArray or IEnumerable;
    }
}