using Prolexy.Compiler.SchemaGenerators;

namespace Prolexy.Compiler.Models;

public record GenericTypeData(string Name) : ITypeData
{
    public TypeCategory Category => TypeCategory.Generic;
}