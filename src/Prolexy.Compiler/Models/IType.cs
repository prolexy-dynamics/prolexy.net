using Newtonsoft.Json.Linq;

namespace Prolexy.Compiler.Models;

public interface IType
{
    IType? GetPropertyType(string name);
}

public class ClrType : IType
{
    private readonly Type _type;

    public ClrType(Type type)
    {
        _type = type;
    }

    public string Name => _type.Name;
    public Type Type => _type;
    public IType? GetPropertyType(string name)
    {
        var prop = _type.GetProperty(name);
        return prop != null ? new ClrType(prop.PropertyType) : null;
    }
}

public class ClrType<T> : ClrType
{
    public ClrType() : base(typeof(T))
    {
    }
}