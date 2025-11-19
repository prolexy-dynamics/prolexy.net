using Prolexy.Compiler.SchemaGenerators;

namespace Prolexy.Compiler.Models;

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

    public Type? ToClrType()
    {
        throw new NotImplementedException();
    }
}