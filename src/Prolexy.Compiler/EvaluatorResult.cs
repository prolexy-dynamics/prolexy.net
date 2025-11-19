using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler;

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