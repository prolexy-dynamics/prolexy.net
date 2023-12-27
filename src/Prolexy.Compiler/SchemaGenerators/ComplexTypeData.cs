using System.Collections.Immutable;

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
}
public enum TypeCategory
{
    Primitive,
    Enum,
    Complex,
    Method,
    Enumerable,
    Generic
}

public record ComplexTypeData : ITypeData 
{
    public ComplexTypeData(string Name, IEnumerable<PropertyData> Peroperties, IEnumerable<MethodData> Methods)
    {
        this.Name = Name;
        this.Peroperties = Peroperties;
        this.Methods = Methods;
    }

    public TypeCategory Category => TypeCategory.Complex;
    public string Name { get; init; }
    public IEnumerable<PropertyData> Peroperties { get; init; }
    public IEnumerable<MethodData> Methods { get; init; }

    public void Deconstruct(out string Name, out IEnumerable<PropertyData> Peroperties, out IEnumerable<MethodData> Methods)
    {
        Name = this.Name;
        Peroperties = this.Peroperties;
        Methods = this.Methods;
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
    public string Name { get; set; }
    public TypeCategory Category => TypeCategory.Method;
    public IEnumerable<ParameterData> Parameters { get; set; }
    public ITypeData ReturnTypeData { get; set; }
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
    public string Name { get; set; }
    public TypeCategory Category => TypeCategory.Enumerable;
}

public record EnumTypeData : ITypeData
{
    public string Name { get; set; }
    public TypeCategory Category => TypeCategory.Enum;
}