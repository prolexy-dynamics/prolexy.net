namespace Prolexy.Compiler.SchemaGenerators;

public class ContextSchema
{
    public ComplexTypeData BusinessObjectTypeData { get; set; }
    public IEnumerable<MethodData> ExtensionMethods { get; set; }
    public IEnumerable<ComplexTypeData> ComplexDataTypes { get; set; }
}