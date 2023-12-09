using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler;

public interface IEvaluatorVisitor
{
    IEvaluatorResult Visit(IAst ast, IEvaluatorContext context);
}

public interface IEvaluatorResult
{
    IEvaluatorContext Context { get; init; }
    object Value { get; init; }
}

public record EvaluatorResult(EvaluatorContext Context, JToken Value) : IEvaluatorResult
{
    IEvaluatorContext IEvaluatorResult.Context
    {
        get => Context;
        init => Context = (EvaluatorContext)value;
    }

    object IEvaluatorResult.Value
    {
        get => Value;
        init => Value = (JToken?)value;
    }
}

public record ClrEvaluatorResult(ClrEvaluatorContext Context, object Value) : IEvaluatorResult
{
    IEvaluatorContext IEvaluatorResult.Context
    {
        get => Context;
        init => Context = (ClrEvaluatorContext)value;
    }
}