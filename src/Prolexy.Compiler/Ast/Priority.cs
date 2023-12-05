using Prolexy.Compiler.Implementations;

namespace Prolexy.Compiler.Ast;

public record Priority(IAst InnerAst, Span Span):IAst
{
    public TR Visit<T, TR>(IAstVisitor<T, TR> visitor, T context)
    {
        return visitor.VisitPriority(this, context);
    }
}