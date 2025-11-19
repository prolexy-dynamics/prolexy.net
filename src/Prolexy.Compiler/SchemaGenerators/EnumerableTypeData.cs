namespace Prolexy.Compiler.SchemaGenerators;

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