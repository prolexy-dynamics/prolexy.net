using Prolexy.Compiler.Implementations;

namespace Prolexy.Compiler.Ast;

public record Assignment(IAst Left, IAst Right, Span Span) : IAst
{
    public TR Visit<T, TR>(IAstVisitor<T, TR> visitor, T context)
    {
        return visitor.VisitAssignment(this, context);
    }
}