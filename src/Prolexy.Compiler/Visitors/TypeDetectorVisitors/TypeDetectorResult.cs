using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.Visitors.TypeDetectorVisitors;

public record TypeDetectorResult(ExpressionTypeDetectorContext Context, Type? Result) : IEvaluatorResult
{
    IEvaluatorContext  IEvaluatorResult.Context
    {
        get => Context;
        init => Context = (ExpressionTypeDetectorContext)value;
    }
    object IEvaluatorResult.Value { get; init; }
}