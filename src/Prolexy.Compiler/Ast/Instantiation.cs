using Prolexy.Compiler.Implementations;

namespace Prolexy.Compiler.Ast;

public record Instantiation(Token Typename, List<IAst> Arguments, Span Span) : IAst
{
    public TR Visit<T, TR>(IAstVisitor<T, TR> visitor, T context)
    {
        return visitor.VisitInstantiation(this, context);
    }
}