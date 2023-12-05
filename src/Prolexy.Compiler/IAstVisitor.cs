using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Models;
using AnonymousMethod = Prolexy.Compiler.Ast.AnonymousMethod;

namespace Prolexy.Compiler;

public interface IAstVisitor<Context, TResult>
{
    TResult VisitBinary(Binary binary, Context context);
    TResult VisitAssignment(Assignment assignment, Context context);
    TResult VisitAccessMember(AccessMember accessMember, Context context);
    TResult VisitImplicitAccessMember(ImplicitAccessMember implicitAccessMember, Context context);
    TResult VisitLiteral(LiteralPrimitive literalPrimitive, Context context);
    TResult VisitStatements(Statement statement, Context context);
    TResult VisitIfStatement(IfStatement ifStatement, Context context);
    TResult VisitMethodCall(Call call, Context context);
    TResult VisitPriority(Priority priority, Context context);
    TResult VisitAnonymousMethod(AnonymousMethod anonymousMethod, Context context);
}

public record EvaluatorResult(EvaluatorContext Context, JToken? Value);