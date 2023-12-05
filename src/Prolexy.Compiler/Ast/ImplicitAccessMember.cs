using Prolexy.Compiler.Implementations;

namespace Prolexy.Compiler.Ast;

public record ImplicitAccessMember(Token Token, Span Span) : IAccessMember
{
    public TR Visit<T, TR>(IAstVisitor<T, TR> visitor, T context)
    {
        return visitor.VisitImplicitAccessMember(this, context);
    }
}