using System.Collections;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.Enumerable;

public record AggregateMethod() : EnumerationExtensionMethod("Aggregate", new Parameter[]
    {
        new("initialState", new GenericType("TState")),
        new("aggregator",
            new MethodSignature(PrimitiveType.Void,
                new Parameter[]
                {
                    new("accumulate", new GenericType("TState")),
                    new("element", new GenericType("TElement"))
                },
                new GenericType("TState")))
    },
    new GenericType("TState"))
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
        if (arguments.Skip(1).First() is not AnonymousMethod predicate)
            throw new ArgumentException("Selector is not Anonymous method.");
        var result = visitor.Visit(arguments[0], context).Value;
        var newScope = new Dictionary<string, object>();
        context.Variables.Push(newScope);
        foreach (var element in items)
        {
            newScope[predicate.Parameters[0].Value!] = result;
            newScope[predicate.Parameters[1].Value!] = element;
            result = visitor.Visit(predicate, context).Value;
        }

        context.Variables.Pop();
        return result;
    }
}