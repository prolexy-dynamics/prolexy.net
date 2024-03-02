using System.Collections.Immutable;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.ExtensionMethods;

namespace Prolexy.Compiler.Models;

public interface IEvaluatorContext
{
    Stack<Dictionary<string, object>>  Variables { get; }
    object BusinessObject { get; init; }
    ImmutableList<Module> Modules { get; init; }
    ImmutableList<Method> ExtensionMethods { get; init; }
}

public record EvaluatorContext(JToken BusinessObject, IType? Schema, ImmutableList<Module> Modules,
    ImmutableList<Method> ExtensionMethods) : IEvaluatorContext
{
    public Stack<Dictionary<string, object>> Variables { get; } = new();
    object IEvaluatorContext.BusinessObject
    {
        get => BusinessObject;
        init => BusinessObject = (JToken)value;
    }
}

public record ClrEvaluatorContext(object BusinessObject, 
    ImmutableList<ClrType> ClrTypes,
    ImmutableList<Module> Modules,
    ImmutableList<Method> ExtensionMethods) : IEvaluatorContext
{
    public Stack<Dictionary<string, object>> Variables { get; } = new();
}

public record SchemaGeneratorEvaluatorContext(Type BusinessObjectType, 
    ImmutableList<ClrType> ClrTypes,
    ImmutableList<Module> Modules,
    ImmutableList<Method> ExtensionMethods) : IEvaluatorContext
{
    public Stack<Dictionary<string, object>> Variables { get; } = new();
    object IEvaluatorContext.BusinessObject { get; init; }
}
