using System.Collections.Immutable;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.SchemaGenerators;

public record ComplexTypeData : ITypeData 
{
    public ComplexTypeData(string Name,
        IEnumerable<PropertyData> Properties,
        IEnumerable<MethodData> Methods,
        IEnumerable<MethodData> Constructors)
    {
        this.Name = Name;
        this.Properties = Properties;
        this.Methods = Methods;
        this.Constructors = Constructors;
    }

    public TypeCategory Category => TypeCategory.Complex;
    public string Name { get; init; }
    public IEnumerable<PropertyData> Properties { get; init; }
    public IEnumerable<MethodData> Methods { get; init; }
    public IEnumerable<MethodData> Constructors { get; init; }

    public void Deconstruct(out string Name,
        out IEnumerable<PropertyData> Properties,
        out IEnumerable<MethodData> Methods,
        out IEnumerable<MethodData> Constructors)
    {
        Name = this.Name;
        Properties = this.Properties;
        Methods = this.Methods;
        Constructors = this.Constructors;
    }
}