using Newtonsoft.Json.Linq;

namespace Prolexy.Compiler.Models;

public interface IType
{
    IType? GetSubType(string name);
    bool Accept(object value);
}