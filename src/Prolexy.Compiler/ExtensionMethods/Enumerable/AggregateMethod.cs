using System.Collections;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.Enumerable;

public record AggregateMethod() : EnumerationExtensionMethod("Aggregate", new Parameter[]
    {
        new("initialState", new GenericType("TA")),
        new("aggregator",
            new MethodSignature(new Parameter[]
                {
                    new("accumulate", new GenericType("TA")),
                    new("element", new GenericType("TE"))
                },
                PrimitiveType.Boolean))
    },
    PrimitiveType.Boolean)
{
    public override object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context,
        object methodContext,
        IEnumerable<IAst> args)
    {
        if (methodContext is not IEnumerable items)
            throw new ArgumentException("Aggregate method can execute on Array types.");
        var arguments = args as IAst[] ?? args.ToArray();
        if (!arguments.Any())
            throw new ArgumentException("Selector not provided for 'Aggregate' method");
        if (arguments.First() is not AnonymousMethod predicate)
            throw new ArgumentException("Selector is not Anonymous method.");
        var result = visitor.Visit(arguments[1], context).Value;
        foreach (var element in items)
        {
            context.Variables[predicate.Parameters[0].Value!] = result;
            context.Variables[predicate.Parameters[1].Value!] = element;
            result = visitor.Visit(predicate, context).Value;
        }

        context.Variables.Remove(predicate.Parameters[0].Value!);
        context.Variables.Remove(predicate.Parameters[1].Value!);
        return result;
    }
}