using Prolexy.Compiler.Implementations;

namespace Prolexy.Compiler.Ast;

public record IfStatement(IAst Condition, Statement ThenStatement, Statement ElseStatement, Span Span) : IAst
{
    public TR Visit<T, TR>(IAstVisitor<T, TR> visitor, T context)
    {
        return visitor.VisitIfStatement(this, context);
    }
}