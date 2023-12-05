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