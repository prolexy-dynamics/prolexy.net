using Newtonsoft.Json.Linq;
using Prolexy.Compiler.SchemaGenerators;

namespace Prolexy.Compiler.Models;

public interface IType
{
    string Name { get; }
    IType? GetPropertyType(string name);
    ITypeData GetTypeData(SchemaGenerator generator);
    Type? ToClrType();
}

public class ClrType<T> : ClrType
{
    public ClrType() : base(typeof(T))
    {
    }
}