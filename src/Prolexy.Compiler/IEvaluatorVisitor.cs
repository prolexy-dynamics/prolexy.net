using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Models;
using AnonymousMethod = Prolexy.Compiler.Ast.AnonymousMethod;

namespace Prolexy.Compiler;

public interface IAstVisitor
{
    IEvaluatorResult Visit(IAst ast, IEvaluatorContext context);
}
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

public interface IEvaluatorResult
{
    IEvaluatorContext Context { get; init; }
    object? Value { get; init; }
}

public record EvaluatorResult(EvaluatorContext Context, JToken? Value) : IEvaluatorResult
{
    IEvaluatorContext IEvaluatorResult.Context
    {
        get => Context;
        init => Context = (EvaluatorContext)value;
    }

    object? IEvaluatorResult.Value
    {
        get => Value;
        init => Value = (JToken?)value;
    }
}

public record ClrEvaluatorResult(ClrEvaluatorContext Context, object? Value) : IEvaluatorResult
{
    IEvaluatorContext IEvaluatorResult.Context
    {
        get => Context;
        init => Context = (ClrEvaluatorContext)value;
    }
}