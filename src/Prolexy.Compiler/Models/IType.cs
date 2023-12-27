using Newtonsoft.Json.Linq;
using Prolexy.Compiler.SchemaGenerators;

namespace Prolexy.Compiler.Models;

public interface IType
{
    string Name { get; }
    IType? GetPropertyType(string name);
    ITypeData GetTypeData(SchemaGenerator generator);
}

public class GenericType : IType
{
    public GenericType(string name)
    {
        Name = name;
    }
    public string Name { get; set; }
    public IType? GetPropertyType(string name)
    {
        throw new NotImplementedException();
    }

    public ITypeData GetTypeData(SchemaGenerator generator)
    {
        return new GenericTypeData(Name);
    }
}

public record GenericTypeData(string Name) : ITypeData
{
    public TypeCategory Category => TypeCategory.Generic;
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

    public ITypeData GetTypeData(SchemaGenerator generator)
    {
        return new SchemaGenerator().FromClrType(_type);
    }
}

public class ClrType<T> : ClrType
{
    public ClrType() : base(typeof(T))
    {
    }
}