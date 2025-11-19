using Prolexy.Compiler.Models;

namespace Prolexy.Compiler;

public record ClrEvaluatorResult(ClrEvaluatorContext Context, object Value) : IEvaluatorResult
{
    IEvaluatorContext IEvaluatorResult.Context
    {
        get => Context;
        init => Context = (ClrEvaluatorContext)value;
    }
}