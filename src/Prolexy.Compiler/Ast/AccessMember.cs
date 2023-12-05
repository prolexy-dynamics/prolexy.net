using Prolexy.Compiler.Implementations;

namespace Prolexy.Compiler.Ast;

public record AccessMember(IAst Left, Token Token, Span Span) : IAccessMember
{
    public TR Visit<T, TR>(IAstVisitor<T, TR> visitor, T context)
    {
        return visitor.VisitAccessMember(this, context);
    }
}