using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.ExtensionMethods;
using Prolexy.Compiler.Models;
using AnonymousMethod = Prolexy.Compiler.Ast.AnonymousMethod;

#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable CS8605
#pragma warning disable CS8604

namespace Prolexy.Compiler.Implementations;

public class EvaluatorVisitor : IAstVisitor<EvaluatorContext, EvaluatorResult>
{
    public EvaluatorResult VisitBinary(Binary binary, EvaluatorContext context)
    {
        EvaluatorResult EvaluatorResult(JToken? value)
        {
            return new EvaluatorResult(context, value);
        }

        var left = (JValue)binary.Left.Visit(this, context).Value;
        var right = (JValue)binary.Right.Visit(this, context).Value;

        switch (binary.Operation)
        {
            case Operations.Is: return EvaluatorResult(Comparer<JValue>.Default.Compare(left, right) == 0);
            case Operations.IsNot: return EvaluatorResult(Comparer<JValue>.Default.Compare(left, right) != 0);
            case Operations.After: return EvaluatorResult((DateTime)left > (DateTime)right);
            case Operations.AfterOrEq: return EvaluatorResult((DateTime)left >= (DateTime)right);
            case Operations.Before: return EvaluatorResult((DateTime)left < (DateTime)right);
            case Operations.BeforeOrEq: return EvaluatorResult((DateTime)left <= (DateTime)right);
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


            case Operations.Eq:
                return EvaluatorResult((decimal?)left == (decimal?)right);
            case Operations.Neq:
                return EvaluatorResult((decimal?)left != (decimal?)right);
            case Operations.Lt:
                return EvaluatorResult((decimal?)left < (decimal?)right);
            case Operations.Lte:
                return EvaluatorResult((decimal?)left <= (decimal?)right);
            case Operations.Gt:
                return EvaluatorResult((decimal?)left > (decimal?)right);
            case Operations.Gte:
                return EvaluatorResult((decimal?)left >= (decimal?)right);

            case Operations.Plus:
                return EvaluatorResult((dynamic)left + (dynamic)right);
            case Operations.Minus:
                return EvaluatorResult((decimal)left - (decimal)right);
            case Operations.Multiply:
                return EvaluatorResult((decimal)left * (decimal)right);
            case Operations.Devide:
                return EvaluatorResult((decimal)left / (decimal)right);
            case Operations.Power:
                return EvaluatorResult(Math.Pow((float)left, (float)right));
            case Operations.Module:
                return EvaluatorResult((decimal)left % (decimal)right);

            case Operations.Or:
                return EvaluatorResult((bool)left || (bool)right);
            case Operations.And:
                return EvaluatorResult((bool)left && (bool)right);
        }

        throw new NotImplementedException();
    }

    public EvaluatorResult VisitAssignment(Assignment assignment, EvaluatorContext context)
    {
        var property = (Property)GetProperty((dynamic)assignment.Left, context.BusinessObject);
        property.Set((dynamic)assignment.Right.Visit(this, context).Value!);
        return new EvaluatorResult(context, null);
    }

    record Property(JToken Context, string Name)
    {
        public void Set(JToken value) => Context[Name] = value;
        public JToken Get() => Context[Name] ?? (Context[Name] = JToken.Parse("{}"));
    }

    private Property GetProperty(ImplicitAccessMember ast, JToken context)
    {
        var prop = ast.Token.Value!;
        return new Property(context, prop);
    }

    private Property GetProperty(AccessMember ast, JToken context)
    {
        var prop = (Property)GetProperty((dynamic)ast.Left, context);
        return new Property(prop.Get(), ast.Token.Value!);
    }

    public EvaluatorResult VisitAccessMember(AccessMember accessMember, EvaluatorContext context)
    {
        var result = accessMember.Left.Visit(this, context);

        if (result.Value is JObject left)
            return new EvaluatorResult(context with { Schema = context.Schema?.GetSubType(accessMember.Token.Value) },
                GetValue(accessMember.Token.Value!, left));
        return result.Context.BusinessObject is JObject jObject
            ? result with
            {
                Context = result.Context with
                {
                    BusinessObject = jObject.GetValue(accessMember.Token.Value) != null
                        ? jObject[accessMember.Token.Value]
                        : JObject.Parse("{}")
                }
            }
            : result;
    }

    public EvaluatorResult VisitImplicitAccessMember(ImplicitAccessMember implicitAccessMember,
        EvaluatorContext context)
    {
        var value = GetValue(implicitAccessMember.Token.Value!, context.BusinessObject);
        return new EvaluatorResult(context with
        {
            Schema = context.Schema?.GetSubType(implicitAccessMember.Token.Value!),
            BusinessObject = context.BusinessObject[implicitAccessMember.Token.Value]
        }, value);
    }

    private static JToken? GetValue(string token, JToken context)
    {
        return context[token];
        // var val = context[token];
        // if (val == null) return null;
        // return val.Type switch
        // {
        //     JTokenType.Float => (decimal)val,
        //     JTokenType.Integer => (decimal)val,
        //     JTokenType.Boolean => (bool)val,
        //     JTokenType.Date => (DateTime)val,
        //     JTokenType.String => (string)val!,
        //     JTokenType.Object => (JObject)val!,
        //     _ => null
        // };
    }

    public EvaluatorResult VisitLiteral(LiteralPrimitive literalPrimitive, EvaluatorContext context)
    {
        EvaluatorResult Result(JToken? value) => new EvaluatorResult(context, value);
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

    public EvaluatorResult VisitStatements(Statement statement, EvaluatorContext context)
    {
        statement.Statements.ForEach(st => st.Visit(this, context));
        return new EvaluatorResult(context, null);
    }

    public EvaluatorResult VisitIfStatement(IfStatement ifStatement, EvaluatorContext context)
    {
        var condResult = ifStatement.Condition.Visit(this, context);
        if (condResult.Value is not null && condResult.Value.Value<bool>())
            ifStatement.ThenStatement.Visit(this, context);
        else
            ifStatement.ElseStatement.Visit(this, context);
        return new EvaluatorResult(context, null);
    }

    public EvaluatorResult VisitMethodCall(Call call, EvaluatorContext context)
    {
        var leftValue = call.MethodSelector.Visit(this, context).Value;
        var method = FindMethod(context, leftValue, call.MethodSelector.Token.Value);
        return new EvaluatorResult(context, method.Eval(this, context, leftValue, call.Arguments));
    }

    private Method FindMethod(EvaluatorContext context, JToken? leftValue, string? methodName)
    {
        Method result = null;
        if (context.Schema is Schema schema)
            result = schema.Methods.FirstOrDefault(m => m.Name == methodName);
        result ??= context.ExtensionMethods
            .FirstOrDefault(m => m.Accept(leftValue) && m.Name == methodName);

        return result;
    }

    public EvaluatorResult VisitPriority(Priority priority, EvaluatorContext context)
    {
        return priority.InnerAst.Visit(this, context);
    }

    public EvaluatorResult VisitAnonymousMethod(AnonymousMethod anonymousMethod, EvaluatorContext context)
    {
        return anonymousMethod.Expression.Visit(this, context);
    }
}