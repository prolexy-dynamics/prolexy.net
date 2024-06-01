using System.Collections.Immutable;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.SchemaGenerators;

public interface ITypeData
{
    string Name { get; }
    TypeCategory Category { get; }
}

public class PrimitiveTypeData : ITypeData
{
    public PrimitiveTypeData(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
    public TypeCategory Category => TypeCategory.Primitive;
}

public class ContextSchema
{
    public ComplexTypeData BusinessObjectTypeData { get; set; }
    public IEnumerable<MethodData> ExtensionMethods { get; set; }
    public IEnumerable<ComplexTypeData> ComplexDataTypes { get; set; }
}
public enum TypeCategory
{
    Primitive,
    Enum,
    Complex,
    Method,
    Enumerable,
    Generic,
    ReferenceType,
    Dynamic
}

public record DynamicTypeData : ITypeData
{
    public string Name { get; } = "Dynamic";
    public TypeCategory Category { get; } = TypeCategory.Dynamic;
}
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

public record PropertyData
{
    public PropertyData(string PropertyName, ITypeData PropertyType)
    {
        this.PropertyName = PropertyName;
        this.PropertyType = PropertyType;
    }

    public string PropertyName { get; set; }
    public ITypeData PropertyType { get; set; }
    
    public void Deconstruct(out string PropertyName, out ITypeData PropertyType)
    {
        PropertyName = this.PropertyName;
        PropertyType = this.PropertyType;
    }
}

public record MethodData : ITypeData
{
    public MethodData(string name, ITypeData contextType, IEnumerable<ParameterData> parameters, ITypeData returnType)
    {
        Name = name;
        ContextType = contextType;
        Parameters = parameters;
        ReturnType = returnType;
    }
    public string Name { get; set; }
    public ITypeData ContextType { get; set; }
    public TypeCategory Category => TypeCategory.Method;
    public IEnumerable<ParameterData> Parameters { get; set; }
    public ITypeData ReturnType { get; set; }
}

public record ParameterData
{
    public ParameterData(string parameterName, ITypeData parameterType)
    {
        ParameterName = parameterName;
        ParameterType = parameterType;
    }
    public string ParameterName { get; set; }
    public ITypeData ParameterType { get; set; }
}

public record EnumerableTypeData : ITypeData
{
    public EnumerableTypeData(ITypeData elementType)
    {
        ElementType = elementType;
    }

    public string Name => $"Enumerable<{ElementType.Name}>";
    public ITypeData ElementType { get; set; }
    public TypeCategory Category => TypeCategory.Enumerable;
}

public record EnumTypeData : ITypeData
{
    public EnumTypeData(string name, IEnumerable<string> items)
    {
        Name = name;
        Items = items;
    }
    public string Name { get; set; }
    public IEnumerable<string> Items { get; set; }
    public TypeCategory Category => TypeCategory.Enum;
}