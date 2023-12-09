using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.ExtensionMethods;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.Implementations;

public interface IEvaluatorVisitor<in T, out TR> : IAstVisitor<T, TR>, IEvaluatorVisitor
    where T : IEvaluatorContext
    where TR : IEvaluatorResult
{
}
#pragma warning disable CS8600, CS8602, CS8605, CS8604
public class ClrEvaluatorVisitor : IEvaluatorVisitor<ClrEvaluatorContext, ClrEvaluatorResult>
{
    public ClrEvaluatorResult VisitBinary(Binary binary, ClrEvaluatorContext context)
    {
        ClrEvaluatorResult EvaluatorResult(object value)
        {
            return new ClrEvaluatorResult(context, value);
        }

        var left = binary.Left.Visit(this, context).Value;
        var right = binary.Right.Visit(this, context).Value;
        if (left != null) right = Convert.ChangeType(right, left.GetType());
        var comparable = left as IComparable;
        switch (binary.Operation)
        {
            case Operations.Eq:
            case Operations.Is:
                return EvaluatorResult(left is IComparable ? comparable.CompareTo(right) == 0 : left == right);
            case Operations.Neq:
            case Operations.IsNot:
                return EvaluatorResult(left is IComparable ? comparable.CompareTo(right) != 0 : left != right);
            case Operations.Gt:
            case Operations.After: return EvaluatorResult((dynamic)left > (dynamic)right);
            case Operations.Gte:
            case Operations.AfterOrEq: return EvaluatorResult((dynamic)left >= (dynamic)right);
            case Operations.Lt:
            case Operations.Before: return EvaluatorResult((dynamic)left < (dynamic)right);
            case Operations.Lte:
            case Operations.BeforeOrEq: return EvaluatorResult((dynamic)left <= (dynamic)right);
            case Operations.Contains:
                return EvaluatorResult(((string)left).Contains((string)right));
            case Operations.NotContains:
                return EvaluatorResult(!((string)left).Contains((string)right));
            case Operations.StartsWith:
                return EvaluatorResult(((string)left).StartsWith((string)right));
            case Operations.NotStartsWith:
                return EvaluatorResult(!((string)left).StartsWith((string)right));
            case Operations.EndsWith:
                return EvaluatorResult(((string)left).EndsWith((string)right));
            case Operations.NotEndsWith:
                return EvaluatorResult(!((string)left).EndsWith((string)right));

            case Operations.Plus:
                return EvaluatorResult((dynamic)left + (dynamic)right);
            case Operations.Minus:
                return EvaluatorResult((dynamic)left - (dynamic)right);
            case Operations.Multiply:
                return EvaluatorResult((dynamic)left * (dynamic)right);
            case Operations.Devide:
                return EvaluatorResult((dynamic)left / (dynamic)right);
            case Operations.Power:
                return EvaluatorResult(Math.Pow((double)left, (double)right));
            case Operations.Module:
                return EvaluatorResult((dynamic)left % (dynamic)right);

            case Operations.Or:
                return EvaluatorResult((bool)left || (bool)right);
            case Operations.And:
                return EvaluatorResult((bool)left && (bool)right);
        }

        throw new NotImplementedException();
    }

    public ClrEvaluatorResult VisitAssignment(Assignment assignment, ClrEvaluatorContext context)
    {
        var setter = (Action<object>)GetProperty((dynamic)assignment.Left, context.BusinessObject);
        setter(assignment.Right.Visit(this, context).Value!);
        return new ClrEvaluatorResult(context, null!);
    }

    private Action<object> GetProperty(ImplicitAccessMember ast, object context)
    {
        var prop = ast.Token.Value!;
        var result = context.GetType().GetProperty(prop);
        if (result == null) throw new PropertyNotFoundException(prop);
        return (val) => result.SetValue(context, Convert.ChangeType(val, result.PropertyType));
    }

    private Action<object> GetProperty(AccessMember ast, object context)
    {
        var ctx = GetPropertyValue(ast.Left, context);
        var property = ctx.GetType().GetProperty(ast.Token.Value);
        return (val) => property.SetValue(ctx, Convert.ChangeType(val, property.PropertyType));
    }

    private object GetPropertyValue(IAst astLeft, object context)
    {
        var property = astLeft is ImplicitAccessMember imAst ? imAst.Token.Value : ((AccessMember)astLeft).Token.Value;
        return context.GetType().GetProperty(property).GetValue(context)!;
    }

    public ClrEvaluatorResult VisitAccessMember(AccessMember accessMember, ClrEvaluatorContext context)
    {
        var result = accessMember.Left.Visit(this, context).Value;
        var property = result.GetType().GetProperty(accessMember.Token.Value);
        if (property != null)
            return new ClrEvaluatorResult(context with { BusinessObject = result }, property.GetValue(result));
        var method = result.GetType().GetMethod(accessMember.Token.Value);
        return new ClrEvaluatorResult(context with { BusinessObject = result }, method);
    }

    public ClrEvaluatorResult VisitImplicitAccessMember(ImplicitAccessMember implicitAccessMember,
        ClrEvaluatorContext context)
    {
        if (context.Variables.TryGetValue(implicitAccessMember.Token.Value, out var variable))
            return new ClrEvaluatorResult(context, variable);
        var property = context.BusinessObject.GetType().GetProperty(implicitAccessMember.Token.Value);
        return new ClrEvaluatorResult(context, property?.GetValue(context.BusinessObject));
    }

    public ClrEvaluatorResult VisitLiteral(LiteralPrimitive literalPrimitive, ClrEvaluatorContext context)
    {
        ClrEvaluatorResult Result(object value) => new ClrEvaluatorResult(context, value);
        var complexLiteralMatch = new Regex(@"^\$\{([\w,\d]+):([\u0600-\u06FF,\w,\s]*):(enum|string|number)\}")
            .Match(literalPrimitive.Token.Value!);

        if (complexLiteralMatch.Success)
        {
            var complexLiteral = complexLiteralMatch.Groups[1].Value;
            if (literalPrimitive.Token.Type is "number" or "enum" && decimal.TryParse(complexLiteral, out var value))
                return Result(value);
            return Result(complexLiteral);
        }

        return literalPrimitive.Token.Type switch
        {
            "string" => Result(literalPrimitive.Token.Value!.Substring(1, literalPrimitive.Token.Value.Length - 2)),
            "datetime" => Result(DateTime.Parse(literalPrimitive.Token.Value!)),
            "number" => Result(decimal.Parse(literalPrimitive.Token.Value!)),
            "boolean" => Result(bool.Parse(literalPrimitive.Token.Value!)),
            _ => Result(null)
        };
    }

    public ClrEvaluatorResult VisitStatements(Statement statement, ClrEvaluatorContext context)
    {
        statement.Statements.ForEach(st => st.Visit(this, context));
        return new ClrEvaluatorResult(context, null);
    }

    public ClrEvaluatorResult VisitIfStatement(IfStatement ifStatement, ClrEvaluatorContext context)
    {
        var condResult = ifStatement.Condition.Visit(this, context);
        if (condResult.Value.Equals(true))
            ifStatement.ThenStatement.Visit(this, context);
        else
            ifStatement.ElseStatement.Visit(this, context);
        return new ClrEvaluatorResult(context, null);
    }

    public ClrEvaluatorResult VisitMethodCall(Call call, ClrEvaluatorContext context)
    {
        var leftValue = call.MethodSelector.Visit(this, context);
        var contextMethod = leftValue.Value as MethodInfo;
        if (contextMethod != null)
        {
            var args = call.Arguments.Select((arg, idx) => Convert.ChangeType(arg.Visit(this, context).Value,
                contextMethod.GetParameters()[idx].ParameterType)).ToArray();
            var result = contextMethod.Invoke(leftValue.Context.BusinessObject, args);
            return new ClrEvaluatorResult(context, result);
        }

        var method = FindMethod(leftValue.Context, leftValue, call);
        return leftValue with
        {
            Value = method.Eval(this, leftValue.Context, leftValue.Context.BusinessObject, call.Arguments)
        };
    }

    private Method FindMethod(ClrEvaluatorContext context, ClrEvaluatorResult clrEvaluatorResult, Call call)
    {
        Method result = null;
        result ??= context.ExtensionMethods
            .FirstOrDefault(m => m.Accept(context.BusinessObject) &&
                                 m.Name == call.MethodSelector.Token.Value);

        return result;
    }

    public ClrEvaluatorResult VisitPriority(Priority priority, ClrEvaluatorContext context)
    {
        return priority.InnerAst.Visit(this, context);
    }

    public ClrEvaluatorResult VisitAnonymousMethod(AnonymousMethod anonymousMethod, ClrEvaluatorContext context)
    {
        return anonymousMethod.Expression.Visit(this, context);
    }

    IEvaluatorResult IEvaluatorVisitor.Visit(IAst ast, IEvaluatorContext context)
    {
        return ast.Visit(this, (ClrEvaluatorContext)context);
    }
}