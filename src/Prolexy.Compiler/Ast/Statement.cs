using Prolexy.Compiler.Implementations;

namespace Prolexy.Compiler.Ast;

public record Statement(List<IAst> Statements, Span Span) : IAst
{
    public TResult Visit<TC, TResult>(IAstVisitor<TC, TResult> visitor, TC context)
    {
        return visitor.VisitStatements(this, context);
    }
}