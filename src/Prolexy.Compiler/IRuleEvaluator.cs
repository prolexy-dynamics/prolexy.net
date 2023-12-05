using Prolexy.Compiler.Models;

namespace Prolexy.Compiler;

public interface IRuleEvaluator<out T>
{
    T? Evaluate(EvaluatorContext evaluatorContext);
}
public interface IRuleEvaluator
{
    void Execute(EvaluatorContext evaluatorContext);
}