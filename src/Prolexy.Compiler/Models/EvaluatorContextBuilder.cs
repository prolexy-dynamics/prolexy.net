using System.Collections.Immutable;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.ExtensionMethods;
using Prolexy.Compiler.ExtensionMethods.DateTimeExtensions;

namespace Prolexy.Compiler.Models;

public record EvaluatorContextBuilder
{
    public static EvaluatorContextBuilder Default => new EvaluatorContextBuilder()
        .ScanAssemblyForExtensionMethod(typeof(AddDaysMethod).Assembly);

    private JObject _businessObject = null!;
    private Schema _schema = null!;
    ImmutableList<Method> _extensionMethods = ImmutableList<Method>.Empty;
    private readonly ImmutableList<Module> _modules = ImmutableList<Module>.Empty;

    public EvaluatorContextBuilder WithBusinessObject(JObject businessObject)
    {
        _businessObject = businessObject;
        return this;
    }

    public EvaluatorContextBuilder WithSchema(Schema schema)
    {
        _schema = schema;
        return this;
    }

    public EvaluatorContextBuilder WithExtensionMethod(Method method)
    {
        _extensionMethods = _extensionMethods.Add(method);
        return this;
    }

    public EvaluatorContextBuilder ScanAssemblyForExtensionMethod(Assembly assembly)
    {
        foreach (var type in assembly.GetExportedTypes()
                     .Where(t => typeof(Method).IsAssignableFrom(t) && !t.IsAbstract))
        {
            if (Activator.CreateInstance(type) is Method method)
                WithExtensionMethod(method);
        }

        return this;
    }

    public EvaluatorContext Build() => new(_businessObject, _schema, _modules, _extensionMethods);
}

public record ClrEvaluatorContextBuilder
{
    public static ClrEvaluatorContextBuilder Default => new ClrEvaluatorContextBuilder()
        .ScanAssemblyForExtensionMethod(typeof(AddDaysMethod).Assembly);

    private object _businessObject = null!;
    ImmutableList<ClrType> _clrTypes = ImmutableList<ClrType>.Empty;
    ImmutableList<Method> _extensionMethods = ImmutableList<Method>.Empty;
    private readonly ImmutableList<Module> _modules = ImmutableList<Module>.Empty;

    public ClrEvaluatorContextBuilder WithBusinessObject(object businessObject)
    {
        _businessObject = businessObject;
        return this;
    }

    public ClrEvaluatorContextBuilder AddClrType<T>()
    {
        _clrTypes = _clrTypes.Add(new ClrType<T>());
        return this;
    }

    public ClrEvaluatorContextBuilder WithExtensionMethod(Method method)
    {
        _extensionMethods = _extensionMethods.Add(method);
        return this;
    }

    public ClrEvaluatorContextBuilder ScanAssemblyForExtensionMethod(Assembly assembly)
    {
        foreach (var type in assembly.GetExportedTypes()
                     .Where(t => typeof(Method).IsAssignableFrom(t) && !t.IsAbstract))
        {
            if (Activator.CreateInstance(type) is Method method)
                WithExtensionMethod(method);
        }

        return this;
    }

    public ClrEvaluatorContext Build() => new(
        _businessObject,
        _clrTypes,
        _modules,
        _extensionMethods);
}