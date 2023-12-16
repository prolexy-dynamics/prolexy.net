using Newtonsoft.Json.Linq;

namespace Prolexy.Compiler.Models;

public interface IType
{
    IType? GetPropertyType(string name);
}

public class ClrType(Type type) : IType
{
    public string Name => type.Name;
    public Type Type => type;
    public IType? GetPropertyType(string name)
    {
        var prop = type.GetProperty(name);
        return prop != null ? new ClrType(prop.PropertyType) : null;
    }
}

public class ClrType<T>() : ClrType(typeof(T))
{
}