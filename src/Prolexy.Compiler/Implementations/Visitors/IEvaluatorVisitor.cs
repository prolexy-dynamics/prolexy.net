using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.Implementations.Visitors;

public interface IEvaluatorVisitor<in T, out TR> : IAstVisitor<T, TR>, IEvaluatorVisitor
    where T : IEvaluatorContext
{
}