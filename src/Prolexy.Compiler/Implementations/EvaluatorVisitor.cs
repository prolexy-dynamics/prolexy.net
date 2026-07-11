using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.ExtensionMethods;
using Prolexy.Compiler.Implementations.Visitors;
using Prolexy.Compiler.Models;
using AnonymousMethod = Prolexy.Compiler.Ast.AnonymousMethod;

#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable CS8605
#pragma warning disable CS8604

namespace Prolexy.Compiler.Implementations;

public class EvaluatorVisitor : IEvaluatorVisitor<EvaluatorContext, EvaluatorResult>
{
    public EvaluatorResult VisitBinary(Binary binary, EvaluatorContext context)
    {
        EvaluatorResult EvaluatorResult(JToken value)
        {
            return new EvaluatorResult(context, value);
        }

        var left = (JValue)binary.Left.Visit(this, context).Value;
        var right = (JValue)binary.Right.Visit(this, context).Value;

        return binary.Operation switch
        {
            Operations.Is or  Operations.DateEqual =>
                EvaluatorResult(Comparer<JValue>.Default.Compare(left, right) == 0),
            Operations.IsNot => EvaluatorResult(Comparer<JValue>.Default.Compare(left, right) != 0),
            Operations.After => EvaluatorResult((DateTime)left > (DateTime)right),
            Operations.AfterOrEq => EvaluatorResult((DateTime)left >= (DateTime)right),
            Operations.Before => EvaluatorResult((DateTime)left < (DateTime)right),
            Operations.BeforeOrEq => EvaluatorResult((DateTime)left <= (DateTime)right),
            Operations.Contains => EvaluatorResult(((string)left).Contains((string)right)),
            Operations.NotContains => EvaluatorResult(!((string)left).Contains((string)right)),
            Operations.StartsWith => EvaluatorResult(((string)left).StartsWith((string)right)),
            Operations.NotStartsWith => EvaluatorResult(!((string)left).StartsWith((string)right)),
            Operations.EndsWith => EvaluatorResult(((string)left).EndsWith((string)right)),
            Operations.NotEndsWith => EvaluatorResult(!((string)left).EndsWith((string)right)),
            Operations.Eq => EvaluatorResult((decimal?)left == (decimal?)right),
            Operations.Neq => EvaluatorResult((decimal?)left != (decimal?)right),
            Operations.Lt => EvaluatorResult((decimal?)left < (decimal?)right),
            Operations.Lte => EvaluatorResult((decimal?)left <= (decimal?)right),
            Operations.Gt => EvaluatorResult((decimal?)left > (decimal?)right),
            Operations.Gte => EvaluatorResult((decimal?)left >= (decimal?)right),
            Operations.Plus => EvaluatorResult((dynamic)left + (dynamic)right),
            Operations.Minus => EvaluatorResult((decimal)left - (decimal)right),
            Operations.Multiply => EvaluatorResult((decimal)left * (decimal)right),
            Operations.Devide => EvaluatorResult((decimal)left / (decimal)right),
            Operations.Power => EvaluatorResult(Math.Pow((float)left, (float)right)),
            Operations.Module => EvaluatorResult((decimal)left % (decimal)right),
            Operations.Or => EvaluatorResult((bool)left || (bool)right),
            Operations.And => EvaluatorResult((bool)left && (bool)right),
            _ => throw new NotImplementedException()
        };
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
            return new EvaluatorResult(context with { Schema = context.Schema?.GetPropertyType(accessMember.Token.Value) },
                GetValue(accessMember.Token.Value!, context, left));
        return result.Context.BusinessObject is JObject jObject
            ? result with
            {
                Context = result.Context with
                {
                    BusinessObject = jObject.GetValue(accessMember.Token.Value) != null
                        ? jObject[accessMember.Token.Value]!
                        : JObject.Parse("{}")
                }
            }
            : result;
    }

    public EvaluatorResult VisitImplicitAccessMember(ImplicitAccessMember implicitAccessMember,
        EvaluatorContext context)
    {
        foreach (var variables in context.Variables)
        {
            if (variables.TryGetValue(implicitAccessMember.Token.Value, out var variable))
                return new EvaluatorResult(context, (JToken)variable);
        }    
        var value = GetValue(implicitAccessMember.Token.Value!, context, context.BusinessObject);
        return new EvaluatorResult(context with
        {
            Schema = context.Schema?.GetPropertyType(implicitAccessMember.Token.Value!),
            BusinessObject = context.BusinessObject[implicitAccessMember.Token.Value]
        }, value);
    }

    private static JToken GetValue(string token,  EvaluatorContext context, JToken businessObject)
    {
        foreach (var variables in context.Variables)
        {
            if (variables.TryGetValue(token, out var variable))
                return (JToken)variable;
        }   
        return  businessObject[token]!;
    }

    public EvaluatorResult VisitLiteral(LiteralPrimitive literalPrimitive, EvaluatorContext context)
    {
        EvaluatorResult Result(JToken value) => new EvaluatorResult(context, value);
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
        var method = FindMethod(context, leftValue, call.MethodSelector.Token.Value, call.MethodSelector is ImplicitAccessMember);
        var result = (dynamic)method.Eval(this, context, leftValue, call.Arguments);
        return new EvaluatorResult(context, (JToken)result);
    }

    private Method FindMethod(EvaluatorContext context, JToken leftValue, string? methodName, bool implicitAccessMethod)
    {
        Method result = null;
        if (context.Schema is Schema schema)
            result = schema.Methods.FirstOrDefault(m => m.Name == methodName);
        result ??= context.ExtensionMethods
            .FirstOrDefault(m => m.Accept(leftValue, implicitAccessMethod) && m.Name == methodName);

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

    public EvaluatorResult VisitInstantiation(Instantiation instantiation, EvaluatorContext context)
    {
        return new EvaluatorResult(context, JToken.Parse("{}"));
    }

    public IEvaluatorResult Visit(IAst ast, IEvaluatorContext context)
    {
        return ast.Visit(this, (EvaluatorContext)context);
    }
}