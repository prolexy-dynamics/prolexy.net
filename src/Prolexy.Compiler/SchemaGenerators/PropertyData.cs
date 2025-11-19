namespace Prolexy.Compiler.SchemaGenerators;

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