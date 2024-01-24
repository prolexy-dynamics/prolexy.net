using System.Collections;
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

    ImmutableList<Method> _extensionMethods = ImmutableList<Method>.Empty;
    private readonly ImmutableList<Module> _modules = ImmutableList<Module>.Empty;

    public EvaluatorContextBuilder WithExtensionMethod(Method method)
    {
         _extensionMethods = _extensionMethods.Add(method);
         return this;
    }

    private Hashtable visitedAssembly = new();
    public EvaluatorContextBuilder ScanAssemblyForExtensionMethod(Assembly assembly)
    {
        if (visitedAssembly.ContainsKey(assembly)) return this;
        visitedAssembly[assembly] = true;
        foreach (var type in assembly.GetExportedTypes()
                     .Where(t => typeof(Method).IsAssignableFrom(t) && !t.IsAbstract))
        {
            if (type.GetConstructors().Any(c => c.GetParameters().Length == 0) &&
                Activator.CreateInstance(type) is Method method)
                WithExtensionMethod(method);
        }

        return this;
    }

    public JsonEvaluatorContextBuilder AsJsonEvaluatorBuilder() => new(_modules, _extensionMethods);
    public ClrEvaluatorContextBuilder AsClrEvaluatorBuilder() => new(_modules, _extensionMethods);
}

public record JsonEvaluatorContextBuilder
{
    private JObject _businessObject = null!;
    private Schema _schema = null!;
    private readonly ImmutableList<Module> _modules;
    private readonly ImmutableList<Method> _extensionMethods;

    internal JsonEvaluatorContextBuilder(ImmutableList<Module> modules, ImmutableList<Method> extensionMethods)
    {
        _modules = modules;
        _extensionMethods = extensionMethods;
    }

    public JsonEvaluatorContextBuilder WithBusinessObject(JObject businessObject)
    {
        return this with { _businessObject = businessObject };
    }

    public JsonEvaluatorContextBuilder WithSchema(Schema schema)
    {
        return this with { _schema = schema };
    }

    public EvaluatorContext Build() => new(_businessObject, _schema, _modules, _extensionMethods);
}

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
        if(_extensionMethods.Find(ext => ext ==method) == null)
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
}