using Prolexy.Compiler.Implementations;

namespace Prolexy.Compiler.Ast;

public record LiteralPrimitive(Token Token, Span Span) : IAstExpression
{
    public TR Visit<T, TR>(IAstVisitor<T, TR> visitor, T context)
    {
        return visitor.VisitLiteral(this, context);
    }
}