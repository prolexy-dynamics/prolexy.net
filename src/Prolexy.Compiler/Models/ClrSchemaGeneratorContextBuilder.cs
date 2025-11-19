using System.Collections.Immutable;
using Prolexy.Compiler.ExtensionMethods;

namespace Prolexy.Compiler.Models;

public class ClrSchemaGeneratorContextBuilder
{
    private readonly Type _businessObjectType;
    private ImmutableList<ClrType> _clrTypes;
    private ImmutableList<Module> _modules;
    private ImmutableList<Method> _extensionMethods;

    public ClrSchemaGeneratorContextBuilder(
        Type businessObjectType,
        ImmutableList<ClrType> clrTypes, 
        ImmutableList<Module> modules,
        ImmutableList<Method> extensionMethods)
    {
        _businessObjectType = businessObjectType;
        _clrTypes = clrTypes;
        _modules = modules;
        _extensionMethods = extensionMethods;
    }

    public SchemaGeneratorEvaluatorContext Build() => new(
        _businessObjectType,
        _clrTypes,
        _modules,
        _extensionMethods);
}