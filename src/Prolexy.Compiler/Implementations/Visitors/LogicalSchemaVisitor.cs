using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.Implementations.Visitors;

public class LogicalSchemaVisitor : IEvaluatorVisitor<ClrEvaluatorContext, LogicalSchemaNode>
{
    private readonly HashSet<string> _lambdaParameters = new();
    private readonly Dictionary<string, Type> _lambdaParameterTypes = new();

    // -----------------------------------------
    //   Visit Root
    // -----------------------------------------

    public LogicalSchemaNode VisitBinary(Binary binary, ClrEvaluatorContext context)
    {
        var left = binary.Left.Visit(this, context);
        var right = binary.Right.Visit(this, context);
        return left.MergeWith(right);
    }

    public LogicalSchemaNode VisitStatements(Statement statement, ClrEvaluatorContext context)
    {
        var result = LogicalSchemaNode.Object();
        return statement.Statements.Aggregate(result, 
            (current, st) => 
                current.MergeWith(st.Visit(this, context)));
    }

    public LogicalSchemaNode VisitLiteral(LiteralPrimitive literalPrimitive, ClrEvaluatorContext context)
        => LogicalSchemaNode.Object();

    public LogicalSchemaNode VisitAssignment(Assignment assignment, ClrEvaluatorContext context)
        => assignment.Left.Visit(this, context)
            .MergeWith( assignment.Right.Visit(this, context));

    public LogicalSchemaNode VisitPriority(Priority priority, ClrEvaluatorContext context)
        => priority.InnerAst.Visit(this, context);

    public LogicalSchemaNode VisitIfStatement(IfStatement ifStatement, ClrEvaluatorContext context)
    {
        var cond = ifStatement.Condition.Visit(this, context);
        var thenPart = ifStatement.ThenStatement.Visit(this, context);
        var elsePart = ifStatement.ElseStatement?.Visit(this, context) ?? LogicalSchemaNode.Object();
        return cond
            .MergeWith(thenPart)
            .MergeWith(elsePart);
    }

    // -----------------------------------------
    //    Access Member
    // -----------------------------------------

    public LogicalSchemaNode VisitImplicitAccessMember(ImplicitAccessMember implicitAccessMember,
        ClrEvaluatorContext context)
    {
        var name = implicitAccessMember.Token.Value;

        // Lambda param → skip
        if (_lambdaParameters.Contains(name))
            return LogicalSchemaNode.Object();

        var prop = context.BusinessObject.GetType().GetProperty(name);

        LogicalSchemaNode leaf;

        if (prop != null)
            leaf = BuildFromType(prop.PropertyType);
        else
            leaf = BuildFromNameGuess(name);

        return Wrap(name, leaf);
    }

    public LogicalSchemaNode VisitAccessMember(AccessMember accessMember, ClrEvaluatorContext context)
    {
        var name = accessMember.Token.Value;

        // Case 1: x.Quantity
        if (accessMember.Left is ImplicitAccessMember imp &&
            _lambdaParameters.Contains(imp.Token.Value))
        {
            var lambdaName = imp.Token.Value;

            if (_lambdaParameterTypes.TryGetValue(lambdaName, out var paramType))
            {
                var p = paramType.GetProperty(name);
                if (p != null)
                    return Wrap(name, BuildFromType(p.PropertyType));
            }

            return Wrap(name, BuildFromNameGuess(name));
        }

        // Case 2: normal path
        var type = GetContextType(accessMember, context.BusinessObject);
        LogicalSchemaNode leaf;

        if (type == typeof(Newtonsoft.Json.Linq.JObject))
            leaf = BuildFromNameGuess(name);
        else if (type != null)
            leaf = BuildFromType(type);
        else
            leaf = BuildFromNameGuess(name);

        var path = ExtractPath(accessMember);
        return BuildNested(path, leaf);
    }

    // -----------------------------------------
    //    MethodCall (Exists, etc.)
    // -----------------------------------------

    public LogicalSchemaNode VisitMethodCall(Call call, ClrEvaluatorContext context)
    {
        var leftAccess = call.MethodSelector as AccessMember;
        var leftType = GetContextType(leftAccess.Left, context.BusinessObject);

        if (call.MethodSelector.Token.Value == "Exists" ||
            call.MethodSelector.Token.Value == "Count" ||
            call.MethodSelector.Token.Value == "Sum" ||
            call.MethodSelector.Token.Value == "Aggregate")
        {
            var inner = LogicalSchemaNode.Object();

            if (call.Arguments.FirstOrDefault() is AnonymousMethod am)
            {
                var elementType = leftType?.IsArray == true
                    ? leftType.GetElementType()
                    : null;

                foreach (var p in am.Parameters)
                {
                    _lambdaParameters.Add(p.Value);
                    if (elementType != null)
                        _lambdaParameterTypes[p.Value] = elementType;
                }

                inner = am.Expression.Visit(this, context);

                foreach (var p in am.Parameters)
                {
                    _lambdaParameters.Remove(p.Value);
                    _lambdaParameterTypes.Remove(p.Value);
                }
            }

            var arrayName = ((ImplicitAccessMember)leftAccess.Left).Token.Value;

            return Wrap(arrayName, LogicalSchemaNode.Array(inner));
        }

        // Default
        var r = call.MethodSelector.Visit(this, context);

        return call.Arguments.Aggregate(r,
            (current, arg) =>
                current.MergeWith(arg.Visit(this, context)));
    }

    public LogicalSchemaNode VisitAnonymousMethod(AnonymousMethod anonymousMethod, ClrEvaluatorContext context)
    {
        foreach (var p in anonymousMethod.Parameters)
            _lambdaParameters.Add(p.Value);

        var r = anonymousMethod.Expression.Visit(this, context);

        foreach (var p in anonymousMethod.Parameters)
            _lambdaParameters.Remove(p.Value);

        return r;
    }

    public LogicalSchemaNode VisitInstantiation(Instantiation instantiation, ClrEvaluatorContext context)
    {
        var r = LogicalSchemaNode.Object();
        return instantiation.Arguments.Aggregate(r,
            (current, a) =>
                current.MergeWith(a.Visit(this, context)));
    }

    IEvaluatorResult IEvaluatorVisitor.Visit(IAst ast, IEvaluatorContext context)
        => throw new NotImplementedException();

    // -----------------------------------------
    // Helpers
    // -----------------------------------------

    private LogicalSchemaNode Wrap(string key, LogicalSchemaNode leaf)
    {
        var obj = LogicalSchemaNode.Object();
        obj.Properties[key] = leaf;
        return obj;
    }

    private List<string> ExtractPath(IAst node)
    {
        var list = new List<string>();

        if (node is ImplicitAccessMember im)
            list.Add(im.Token.Value);
        else if (node is AccessMember am)
        {
            list.AddRange(ExtractPath(am.Left));
            list.Add(am.Token.Value);
        }

        return list;
    }

    private LogicalSchemaNode BuildNested(IEnumerable<string> path, LogicalSchemaNode leaf)
    {
        var current = leaf;

        foreach (var p in path.Reverse())
        {
            var obj = LogicalSchemaNode.Object();
            obj.Properties[p] = current;
            current = obj;
        }

        return current;
    }

    private LogicalSchemaNode BuildFromType(Type t)
    {
        if (t == typeof(string)) return LogicalSchemaNode.Primitive("string");
        if (t == typeof(bool)) return LogicalSchemaNode.Primitive("boolean");
        if (t == typeof(int) || t == typeof(decimal) || t == typeof(double))
            return LogicalSchemaNode.Primitive("number");
        if (t == typeof(DateTime)) return LogicalSchemaNode.Primitive("date-time");
        if (t == typeof(Newtonsoft.Json.Linq.JObject))
            return LogicalSchemaNode.Object();

        if (t.IsArray)
            return LogicalSchemaNode.Array(BuildFromType(t.GetElementType()));

        // custom type → object properties
        var obj = LogicalSchemaNode.Object();
        foreach (var p in t.GetProperties())
            obj.Properties[p.Name] = BuildFromType(p.PropertyType);

        return obj;
    }

    private LogicalSchemaNode BuildFromNameGuess(string name)
    {
        var lower = name.ToLowerInvariant();

        if (lower.Contains("date") ||
            lower.Contains("time") ||
            lower.Contains("dob") ||
            lower.Contains("day") ||
            lower.Contains("birth"))
            return LogicalSchemaNode.Primitive("date-time");

        if (name.Contains("Age") ||
            name.Contains("Amount") ||
            name.Contains("Count") ||
            name.Contains("Number") ||
            name.Contains("Price") ||
            name.Contains("Quantity"))
            return LogicalSchemaNode.Primitive("number");

        if (lower.StartsWith("is") || lower.StartsWith("has"))
            return LogicalSchemaNode.Primitive("boolean");

        return LogicalSchemaNode.Primitive("string");
    }

    private static Type? GetContextType(IAst ast, object business)
    {
        if (ast is ImplicitAccessMember im)
            return business.GetType().GetProperty(im.Token.Value)?.PropertyType;

        if (ast is AccessMember am)
        {
            var p = GetContextType(am.Left, business);
            return p?.GetProperty(am.Token.Value)?.PropertyType;
        }

        return null;
    }
}