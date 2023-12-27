using Newtonsoft.Json.Linq;
using Prolexy.Compiler.ExtensionMethods;
using Prolexy.Compiler.SchemaGenerators;

namespace Prolexy.Compiler.Models;

public record Schema(string Name, Property[] Properties, Method[] Methods) : IType
{
    public IType? GetPropertyType(string name)
    {
        return Properties.FirstOrDefault(p => p.Name == name) as IType ?? Methods.FirstOrDefault(m => m.Name == name);
    }

    public ITypeData GetTypeData(SchemaGenerator generator)
    {
        return new ComplexTypeData(Name,
            Properties.Select(p => new PropertyData(p.Name, p.PropertyType.GetTypeData(generator))),
            Methods.Select(m => new MethodData
            {
                Name = m.Name,
                Parameters = m.Parameters.Select(p =>
                    new ParameterData(p.ParameterName, p.ParameterType.GetTypeData(generator))),
                ReturnTypeData = m.ReturnType.GetTypeData(generator)
            }));
    }

    public bool Accept(object value)
    {
        return false;
    }
}