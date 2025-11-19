namespace Prolexy.Compiler.SchemaGenerators;

public class ComplexTypeReferenceDataType : ITypeData
{
    public ComplexTypeReferenceDataType(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
    public TypeCategory Category => TypeCategory.ReferenceType;
}