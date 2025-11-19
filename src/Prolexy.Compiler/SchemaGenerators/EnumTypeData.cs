namespace Prolexy.Compiler.SchemaGenerators;

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