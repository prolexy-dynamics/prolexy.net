using Prolexy.Compiler.Ast;

namespace Prolexy.Compiler;

public interface IAstVisitor<in TContext, out TResult>
{
    TResult VisitBinary(Binary binary, TContext context);
    TResult VisitAssignment(Assignment assignment, TContext context);
    TResult VisitAccessMember(AccessMember accessMember, TContext context);
    TResult VisitImplicitAccessMember(ImplicitAccessMember implicitAccessMember, TContext context);
    TResult VisitLiteral(LiteralPrimitive literalPrimitive, TContext context);
    TResult VisitStatements(Statement statement, TContext context);
    TResult VisitIfStatement(IfStatement ifStatement, TContext context);
    TResult VisitMethodCall(Call call, TContext context);
    TResult VisitPriority(Priority priority, TContext context);
    TResult VisitAnonymousMethod(AnonymousMethod anonymousMethod, TContext context);
}