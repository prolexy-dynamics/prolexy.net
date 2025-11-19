using System.Collections.Immutable;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.ExtensionMethods;

namespace Prolexy.Compiler.Models;

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