using System.Collections.Immutable;
using System.Reflection;
using Prolexy.Compiler.ExtensionMethods;
using Prolexy.Compiler.ExtensionMethods.DateTimeExtensions;
using Prolexy.Compiler.Visitors.TypeDetectorVisitors;

namespace Prolexy.Compiler.Models;

public record ExpressionTypeDetectorContextBuilder
{
    private object _businessObject;
    ImmutableList<ClrType> _clrTypes;
    ImmutableList<Method> _extensionMethods;
    private readonly ImmutableList<Module> _modules;

    internal ExpressionTypeDetectorContextBuilder(object businessObject, 
        ImmutableList<ClrType> clrTypes,
        ImmutableList<Method> extensionMethods,
        ImmutableList<Module> modules)
    {
        _businessObject = businessObject;
        _clrTypes = clrTypes;
        _extensionMethods = extensionMethods;
        _modules = modules;
    }

    private Type _businessObjectType;
    public ExpressionTypeDetectorContextBuilder WithBusinessObjectType(Type businessObjectType) =>
        this with { _businessObjectType =  businessObjectType};
    public ExpressionTypeDetectorContext Build() => new(
        _businessObjectType,
        _modules,
        _extensionMethods,
        _clrTypes);
}