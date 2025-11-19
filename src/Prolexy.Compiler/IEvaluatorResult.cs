using Prolexy.Compiler.Models;

namespace Prolexy.Compiler;

public interface IEvaluatorResult
{
    IEvaluatorContext Context { get; init; }
    object Value { get; init; }
}