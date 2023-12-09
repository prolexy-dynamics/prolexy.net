using Prolexy.Compiler.Models;

namespace Prolexy.Compiler;

public interface IRuleEvaluator<in TC, out T> where TC : IEvaluatorContext 
{
    T? Evaluate(TC evaluatorContext);
}
public interface IRuleEvaluator<in TC> where TC : IEvaluatorContext
{
    void Execute(TC evaluatorContext);
}