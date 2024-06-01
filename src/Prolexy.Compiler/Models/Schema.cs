using Newtonsoft.Json.Linq;
using Prolexy.Compiler.ExtensionMethods;
using Prolexy.Compiler.SchemaGenerators;

namespace Prolexy.Compiler.Models;

public record Dynamic : IType
{
    private Dynamic()
    {
        
    }
    public static readonly Dynamic Instance = new Dynamic();
    public string Name { get; } = "Dynamic";
    public IType? GetPropertyType(string name)
    {
        return this;
    }

    public ITypeData GetTypeData(SchemaGenerator generator)
    {
        return new DynamicTypeData();
    }
}
public record Schema(string Name, Property[] Properties, Method[] Methods, Method[] Constructors) : IType
{
    public IType? GetPropertyType(string name)
    {
        return Properties.FirstOrDefault(p => p.Name == name) as IType ?? Methods.FirstOrDefault(m => m.Name == name);
    }

    public ITypeData GetTypeData(SchemaGenerator generator)
    {
        return new ComplexTypeData(Name,
            Properties.Select(p => new PropertyData(p.Name, p.PropertyType.GetTypeData(generator))),
            Methods.Select(m => new MethodData(m.Name,
                m.ContextType.GetTypeData(generator),
                m.Parameters.Select(p =>
                    new ParameterData(p.ParameterName, p.ParameterType.GetTypeData(generator))),
                m.ReturnType.GetTypeData(generator)
            )), 
            Constructors.Select(c => (MethodData)c.GetTypeData(generator)));
    }

    public bool Accept(object value)
    {
        return false;
    }
}