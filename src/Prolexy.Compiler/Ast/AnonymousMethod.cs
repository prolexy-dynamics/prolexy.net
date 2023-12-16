using Prolexy.Compiler.Implementations;

namespace Prolexy.Compiler.Ast;

public record AnonymousMethod(List<Token> Parameters, IAst Expression, Span Span) : IAst
{
    public TR Visit<T, TR>(IAstVisitor<T, TR> visitor, T context)
    {
        return visitor.VisitAnonymousMethod(this, context);
    }
}