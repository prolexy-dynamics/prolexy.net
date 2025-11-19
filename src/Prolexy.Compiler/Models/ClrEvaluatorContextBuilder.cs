using System.Collections.Immutable;
using Prolexy.Compiler.ExtensionMethods;

namespace Prolexy.Compiler.Models;

public record ClrEvaluatorContextBuilder
{
    private object _businessObject = null!;
    ImmutableList<ClrType> _clrTypes = ImmutableList<ClrType>.Empty;
    private ImmutableList<Method> _extensionMethods;
    private readonly ImmutableList<Module> _modules;

    public ClrEvaluatorContextBuilder(ImmutableList<Module> modules, ImmutableList<Method> extensionMethods)
    {
        _modules = modules;
        _extensionMethods = extensionMethods;
    }

    public ClrEvaluatorContextBuilder WithBusinessObject(object businessObject)
    {
        return this with { _businessObject = businessObject };
    }

    public ClrEvaluatorContextBuilder WithExtensionMethod(Method method)
    {
        if (_extensionMethods.Find(ext => ext == method) == null)
            _extensionMethods = _extensionMethods.Add(method);
        return this;
    }

    public ClrEvaluatorContextBuilder AddClrType<T>()
    {
        _clrTypes = _clrTypes.Add(new ClrType<T>());
        return this;
    }

    public ExpressionTypeDetectorContextBuilder AsExpressionTypeDetectorContextBuilder() =>
        new(_businessObject, _clrTypes, _extensionMethods, _modules);

    public ClrEvaluatorContext Build() => new(
        _businessObject,
        _clrTypes,
        _modules,
        _extensionMethods);

    public ClrSchemaGeneratorContextBuilder AsSchemaGeneratorContextBuilder()
    {
        return new(_businessObject as Type ?? _businessObject.GetType(), _clrTypes, _modules, _extensionMethods);
    }
    public ClrSchemaGeneratorContextBuilder AsSchemaGeneratorContextBuilder<T>()
    {
        return new(typeof(T), _clrTypes, _modules, _extensionMethods);
    }
}