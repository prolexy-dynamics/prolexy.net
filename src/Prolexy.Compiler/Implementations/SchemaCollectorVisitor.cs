using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.Implementations;

public class SchemaCollectorVisitor : IAstVisitor<ClrEvaluatorContext, SchemaCollectorResult>, IEvaluatorVisitor
{
    public SchemaCollectorResult VisitBinary(Binary binary, ClrEvaluatorContext context)
    {
        var left = binary.Left.Visit(this, context);
        var right = binary.Right.Visit(this, context);

        // سعی می‌کنیم نوع literal سمت راست را برای فیلد سمت چپ اعمال کنیم
        var literalType = InferLiteralType(binary.Right);
        ApplyTypeToLeaf(left.Fields, literalType);

        return Merge(left, right);
    }

    public SchemaCollectorResult VisitAccessMember(AccessMember accessMember, ClrEvaluatorContext context)
    {
        var left = accessMember.Left.Visit(this, context);
        var key = accessMember.Token.Value;

        if (left.Fields.Count == 0)
        {
            return new SchemaCollectorResult(context, new Dictionary<string, object>
            {
                [key] = "string"
            });
        }
        var (leaf, _) = GetLeaf(left);
        // اگر در سمت چپ آبجکت داشتیم، درونش key جدید اضافه می‌کنیم
        //var result = new Dictionary<string, object>();
        foreach (var kv in leaf)
        {
            if (kv.Value is Dictionary<string, object> dict)
                dict[key] = "string";
            else if (kv.Value is List<Dictionary<string, object>> list)
                list.Add(new Dictionary<string, object> { [key] = "string" });
            else
                leaf[kv.Key] = new Dictionary<string, object> { [key] = "string" };
        }

        return left;
    }

    private static (Dictionary<string, object> leaf, Dictionary<string, object>? parent) GetLeaf(SchemaCollectorResult left)
    {
        var leaf = left.Fields;
        Dictionary<string, object>? parent = null;
        while (leaf.Values.FirstOrDefault() is Dictionary<string, object> next)
        {
            parent = leaf;
            leaf = next;
        }

        return (leaf, parent);
    }

    public SchemaCollectorResult VisitImplicitAccessMember(ImplicitAccessMember implicitAccessMember,
        ClrEvaluatorContext context)
    {
        return new SchemaCollectorResult(context, new Dictionary<string, object>
        {
            [implicitAccessMember.Token.Value] = "string"
        });
    }

    public SchemaCollectorResult VisitLiteral(LiteralPrimitive literalPrimitive, ClrEvaluatorContext context)
    {
        return new SchemaCollectorResult(context, new Dictionary<string, object>());
    }

    public SchemaCollectorResult VisitStatements(Statement statement, ClrEvaluatorContext context)
    {
        var result = new SchemaCollectorResult(context, new Dictionary<string, object>());
        foreach (var st in statement.Statements)
            result = Merge(result, st.Visit(this, context));
        return result;
    }

    public SchemaCollectorResult VisitAssignment(Assignment assignment, ClrEvaluatorContext context)
    {
        return assignment.Right.Visit(this, context);
    }

    public SchemaCollectorResult VisitIfStatement(IfStatement ifStatement, ClrEvaluatorContext context)
    {
        var cond = ifStatement.Condition.Visit(this, context);
        var thenPart = ifStatement.ThenStatement.Visit(this, context);
        var elsePart = ifStatement.ElseStatement?.Visit(this, context) ??
                       new(context, new());
        return Merge(Merge(cond, thenPart), elsePart);
    }

    public SchemaCollectorResult VisitPriority(Priority priority, ClrEvaluatorContext context)
    {
        return priority.InnerAst.Visit(this, context);
    }

    public SchemaCollectorResult VisitMethodCall(Call call, ClrEvaluatorContext context)
    {
        var result = new SchemaCollectorResult(context, new Dictionary<string, object>());
        var methodName = call.MethodSelector.Token.Value;

        // بررسی اگر متد از نوع مجموعه‌ای است (Exists, Count, Sum, ...)
        if (IsCollectionMethod(methodName))
        {
            // سمت چپ متد باید یک آرایه باشد (مثل LineItems)
            var m = call.MethodSelector.Visit(this, context);
            var collection = makeMethodSelector(m);

            // حالا وارد lambda می‌شویم
            var lambda = call.Arguments.FirstOrDefault(a => a is AnonymousMethod) as AnonymousMethod;
            if (lambda != null)
            {
                var arraySchema = lambda.Visit(this, context).Fields;
                arraySchema = arraySchema.Values.FirstOrDefault() as Dictionary<string, object>;
                var (leaf, parent) = GetLeaf(collection);
                foreach (var key in (parent ?? leaf).Keys.ToList())
                {
                    (parent ?? leaf)[key] = new List<Dictionary<string, object>> { arraySchema };
                }
            }
            result = Merge(result, collection);
        }
        else
        {
            result = Merge(result, call.MethodSelector.Visit(this, context));
            foreach (var arg in call.Arguments)
                result = Merge(result, arg.Visit(this, context));
        }

        return result;
    }

    private static bool IsCollectionMethod(string name)
    {
        return new[] { "Exists", "Count", "Any", "Sum", "Aggregate", "Min", "Max" }.Contains(name);
    }

    private SchemaCollectorResult makeMethodSelector(SchemaCollectorResult methodSelector)
    {
        var leaf = (Dictionary<string, object>)methodSelector.Fields.Values.First();
        while (leaf.Values.FirstOrDefault() is Dictionary<string, object> next)
        {
            leaf = next;
        }
        leaf.Clear();
        return methodSelector;
    }

    public SchemaCollectorResult VisitAnonymousMethod(AnonymousMethod anonymousMethod, ClrEvaluatorContext context)
    {
        return  anonymousMethod.Expression.Visit(this, context);
    }

    public SchemaCollectorResult VisitInstantiation(Instantiation instantiation, ClrEvaluatorContext context)
    {
        var result = new SchemaCollectorResult(context, new Dictionary<string, object>());
        foreach (var arg in instantiation.Arguments)
            result = Merge(result, arg.Visit(this, context));
        return result;
    }

    IEvaluatorResult IEvaluatorVisitor.Visit(IAst ast, IEvaluatorContext context)
    {
        // Not used — our visitor returns FieldCollectorResult, not IEvaluatorResult
        return ast.Visit(this, (ClrEvaluatorContext)context);
    }


    private static string InferLiteralType(IAst ast)
    {
        if (ast is LiteralPrimitive literal)
        {
            return literal.Token.Type switch
            {
                "string" => "string",
                "number" => "number",
                "boolean" => "boolean",
                "datetime" => "datetime",
                _ => "unknown"
            };
        }

        return "string";
    }

    private static void ApplyTypeToLeaf(Dictionary<string, object> dict, string type)
    {
        foreach (var key in dict.Keys.ToList())
        {
            if (dict[key] is string)
                dict[key] = type;
            else if (dict[key] is Dictionary<string, object> sub)
                ApplyTypeToLeaf(sub, type);
        }
    }

    private static SchemaCollectorResult Merge(SchemaCollectorResult a, SchemaCollectorResult b)
    {
        var result = Merge(a.Fields, b.Fields);
        return a with { Fields = result };
    }

    private static Dictionary<string, object> Merge(Dictionary<string, object> a, Dictionary<string, object> b)
    {
        var result = new Dictionary<string, object>(a);
        foreach (var kv in b)
        {
            if (!result.ContainsKey(kv.Key))
                result[kv.Key] = kv.Value;
            else if (result[kv.Key] is Dictionary<string, object> da && kv.Value is Dictionary<string, object> db)
                result[kv.Key] = Merge(da, db);
        }

        return result;
    }
}