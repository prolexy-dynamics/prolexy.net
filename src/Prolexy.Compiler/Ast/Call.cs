using Prolexy.Compiler.Implementations;

namespace Prolexy.Compiler.Ast;

public record Call(IAccessMember MethodSelector, List<IAst> Arguments, Span Span) : IAst
{
    public TR Visit<T, TR>(IAstVisitor<T, TR> visitor, T context)
    {
        return visitor.VisitMethodCall(this, context);
    }
}