using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.Visitors.TypeDetectorVisitors;

public class ClrExpressionTypeDetectorVisitor : IExpressionTypeDetectorVisitor, IEvaluatorVisitor
{
    public TypeDetectorResult Visit(IAst ast, ExpressionTypeDetectorContext context)
    {
        return ast.Visit(this, context);
    }

    IEvaluatorResult IEvaluatorVisitor.Visit(IAst ast, IEvaluatorContext context)
    {
        return ast.Visit(this, (ExpressionTypeDetectorContext)context);
    }

    public TypeDetectorResult VisitBinary(Binary binary, ExpressionTypeDetectorContext context)
    {
        var left = binary.Left.Visit(this, context);
        var right = binary.Right.Visit(this, context);
        var result = new TypeDetectorResult(context, typeof(void));
        if (Operations.LogicalOperations.Contains(binary.Operation))
            return result with { Result = typeof(bool) };
        if (isNumeric(left.Result))
            return NumericTypeDetectorResult();
        if (left.Result?.IsAssignableTo(typeof(string)) ?? false)
            return StringTypeDetectorResult();
        if (left.Result?.IsAssignableTo(typeof(DateTime?)) ?? false)
            return DatetimeTypeDetectorResult();

        return result;

        TypeDetectorResult NumericTypeDetectorResult()
        {
            if (right.Result == typeof(string) && binary.Operation == Operations.Plus)
                return result with { Result = typeof(string) };
            if (Operations.LogicalOperations.Contains(binary.Operation) ||
                Operations.RelationalOperations.Contains(binary.Operation))
                return result with { Result = typeof(bool) };
            if (Operations.NumericOperations.Contains(binary.Operation))
                return result with { Result = typeof(decimal) };
            return result;
        }

        TypeDetectorResult StringTypeDetectorResult()
        {
            if (Operations.StringOperations.Contains(binary.Operation) ||
                Operations.RelationalOperations.Contains(binary.Operation))
                return result with { Result = typeof(bool) };
            if (binary.Operation == Operations.Plus)
                return result with { Result = typeof(string) };
            return result;
        }

        TypeDetectorResult DatetimeTypeDetectorResult()
        {
            if (Operations.DateOperations.Contains(binary.Operation) ||
                Operations.RelationalOperations.Contains(binary.Operation))
                return result with { Result = typeof(bool) };
            return result;
        }
    }

    private bool isNumeric(Type type) =>
        type == typeof(Int16) ||
        type == typeof(Int32) ||
        type == typeof(Int64) ||
        type == typeof(decimal) ||
        type == typeof(short) ||
        type == typeof(float) ||
        type == typeof(double) ||
        type == typeof(byte);

    public TypeDetectorResult VisitAssignment(Assignment assignment, ExpressionTypeDetectorContext context)
    {
        throw new NotImplementedException();
    }

    public TypeDetectorResult VisitAccessMember(AccessMember accessMember, ExpressionTypeDetectorContext context)
    {
        var leftResult = accessMember.Left.Visit(this, context);
        var property = leftResult.Result?.GetProperty(accessMember.Token.Value);
        if (property != null)
            return new(context, property.PropertyType);
        return leftResult;
        // return new (context with { BusinessObject = leftResult },
        //     new MethodSelector(leftResult, accessMember.Token.Value));
    }

    public TypeDetectorResult VisitImplicitAccessMember(ImplicitAccessMember implicitAccessMember,
        ExpressionTypeDetectorContext context)
    {
        foreach (var variables in context.Variables)
        {
            if (variables.TryGetValue(implicitAccessMember.Token.Value, out var variable))
                return new TypeDetectorResult(context, variable);
        }

        var property = context.BusinessObjectType.GetProperty(implicitAccessMember.Token.Value);

        return new TypeDetectorResult(context, property?.PropertyType ?? context.BusinessObjectType);
    }

    public TypeDetectorResult VisitLiteral(LiteralPrimitive literalPrimitive, ExpressionTypeDetectorContext context)
    {
        return new TypeDetectorResult(context, literalPrimitive.Token.Type switch
        {
            "string" => typeof(string),
            "datetime" => typeof(DateTime),
            "boolean" => typeof(bool),
            "number" => typeof(decimal),
            "object" => typeof(Nullable)
        });
    }

    public TypeDetectorResult VisitStatements(Statement statement, ExpressionTypeDetectorContext context)
    {
        throw new NotImplementedException();
    }

    public TypeDetectorResult VisitIfStatement(IfStatement ifStatement, ExpressionTypeDetectorContext context)
    {
        throw new NotImplementedException();
    }

    public TypeDetectorResult VisitMethodCall(Call call, ExpressionTypeDetectorContext context)
    {
        var left = call.MethodSelector.Visit(this, context);
        if (!call.Arguments.Any(a => a is AnonymousMethod))
        {
            var argTypes = call.Arguments.Select(a => a.Visit(this, context).Result).ToArray();
            var method = (left.Result?.GetMethods())
                .SingleOrDefault(m => m.Name == call.MethodSelector.Token.Value &&
                                      m.GetParameters().Length == argTypes.Length &&
                                      m.GetParameters().Where((p, i) =>
                                          (isNumeric(p.ParameterType) && isNumeric(argTypes[i])) ||
                                          p.ParameterType.IsAssignableFrom(argTypes[i])).Count() == argTypes.Length);
            if (method != null)
                return left with { Result = method?.ReturnType };
        }

        var extMethod = context.ExtensionMethods.SingleOrDefault(m => m.Name == call.MethodSelector.Token.Value);
        if (extMethod?.ReturnType is PrimitiveType primitive)
            return left with { Result = primitive.ToType() };
        if (extMethod?.ReturnType is ClrType clrType)
            return left with { Result = clrType.Type };
        if (extMethod?.ReturnType is GenericType generic)
        {
            var generics = new Dictionary<string, Type>();
            if (extMethod.ContextType is EnumerableType enumerable)
                generics[enumerable.ElementType.Name] = left.Result
                    .GetInterfaces()
                    .SingleOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    .GetGenericArguments()[0];
            int index = 0;
            foreach (var argument in call.Arguments)
            {
                var x = argument.Visit(this, context);
                if (extMethod.Parameters.Skip(index++).First().ParameterType is GenericType pType)
                    generics.TryAdd(pType.Name, x.Result!);
            }

            if (generics.ContainsKey(generic.Name))
                return left with { Result = generics[generic.Name] };
        }

        return null;
    }

    public TypeDetectorResult VisitPriority(Priority priority, ExpressionTypeDetectorContext context)
    {
        return priority.InnerAst.Visit(this, context);
    }

    public TypeDetectorResult VisitAnonymousMethod(AnonymousMethod anonymousMethod,
        ExpressionTypeDetectorContext context)
    {
        return new(context, null);
    }

    public TypeDetectorResult VisitInstantiation(Instantiation instantiation, ExpressionTypeDetectorContext context)
    {
        return new(context, context.ClrTypes.Find(t => t.Type.Name == instantiation.Typename.Value)?.Type);
    }
}