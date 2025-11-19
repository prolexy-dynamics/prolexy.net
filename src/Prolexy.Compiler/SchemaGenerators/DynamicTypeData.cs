namespace Prolexy.Compiler.SchemaGenerators;

public record DynamicTypeData : ITypeData
{
    public string Name { get; } = "Dynamic";
    public TypeCategory Category { get; } = TypeCategory.Dynamic;
}