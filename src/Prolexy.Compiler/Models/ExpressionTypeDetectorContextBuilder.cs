using System.Collections.Immutable;
using System.Reflection;
using Prolexy.Compiler.ExtensionMethods;
using Prolexy.Compiler.ExtensionMethods.DateTimeExtensions;
using Prolexy.Compiler.Visitors.TypeDetectorVisitors;

namespace Prolexy.Compiler.Models;

public record ExpressionTypeDetectorContextBuilder
{
    public static ExpressionTypeDetectorContextBuilder Default => new ExpressionTypeDetectorContextBuilder()
        .ScanAssemblyForExtensionMethod(typeof(AddDaysMethod).Assembly);

    private object _businessObject = null!;
    ImmutableList<Type> _clrTypes = ImmutableList<Type>.Empty;
    ImmutableList<Method> _extensionMethods = ImmutableList<Method>.Empty;
    private readonly ImmutableList<Module> _modules = ImmutableList<Module>.Empty;

    public ExpressionTypeDetectorContextBuilder WithBusinessObject(object businessObject)
    {
        _businessObject = businessObject;
        return this;
    }

    public ExpressionTypeDetectorContextBuilder AddClrType<T>()
    {
        _clrTypes = _clrTypes.Add(typeof(T));
        return this;
    }

    public ExpressionTypeDetectorContextBuilder WithExtensionMethod(Method method)
    {
        var old = _extensionMethods.Find(m => m.Name == method.Name);
        _extensionMethods = old != null
            ? _extensionMethods.Replace(old, method)
            : _extensionMethods.Add(method);
        return this;
    }

    public ExpressionTypeDetectorContextBuilder ScanAssemblyForExtensionMethod(Assembly assembly)
    {
        foreach (var type in assembly.GetExportedTypes()
                     .Where(t => typeof(Method).IsAssignableFrom(t) && !t.IsAbstract))
        {
            if (type.GetConstructors().Any(c => c.GetParameters().Length == 0) &&
                Activator.CreateInstance(type) is Method method)
                WithExtensionMethod(method);
        }

        return this;
    }

    public ExpressionTypeDetectorContext Build() => new(
        _businessObject,
        _modules,
        _extensionMethods,
        _clrTypes);
}