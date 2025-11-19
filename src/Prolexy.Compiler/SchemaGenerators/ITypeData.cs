namespace Prolexy.Compiler.SchemaGenerators;

public interface ITypeData
{
    string Name { get; }
    TypeCategory Category { get; }
}