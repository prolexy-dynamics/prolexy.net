namespace Prolexy.Compiler.SchemaGenerators;

public class PrimitiveTypeData : ITypeData
{
    public PrimitiveTypeData(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
    public TypeCategory Category => TypeCategory.Primitive;
}