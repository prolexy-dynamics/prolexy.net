using Prolexy.Compiler.Models;

namespace Prolexy.Compiler;

public interface IVisitorFactory
{
    IAstVisitor<EvaluatorContext, EvaluatorResult> CreateEvaluator();
}