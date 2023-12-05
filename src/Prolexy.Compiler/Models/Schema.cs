using Newtonsoft.Json.Linq;
using Prolexy.Compiler.ExtensionMethods;

namespace Prolexy.Compiler.Models;

public record Schema(Property[] Properties, Method[] Methods) : IType
{
    public IType? GetSubType(string name)
    {
        return Properties.FirstOrDefault(p => p.Name == name) as IType ?? Methods.FirstOrDefault(m => m.Name == name);
    }

    public bool Accept(JToken? value)
    {
        return false;
    }
}