using System.Collections;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.Enumerable;

public record SumMethod() : EnumerationExtensionMethod("Sum",
    new Parameter[]
    {
        new("selector",
            new MethodSignature(new Parameter[]
                {
                    new("element", new GenericType("T"))
                },
                PrimitiveType.Number))
    },
    PrimitiveType.Number)
{
    public override object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context,
        object methodContext,
        IEnumerable<IAst> args)
    {
        if (methodContext is not IEnumerable items)
            throw new ArgumentException("Sum method can execute on Array types.");
        var arguments = args as IAst[] ?? args.ToArray();
        if (!arguments.Any())
            throw new ArgumentException("Selector not provided for 'Sum' method");
        if (arguments.First() is not AnonymousMethod predicate)
            throw new ArgumentException("Selector is not Anonymous method.");
        var sum = new decimal(0);
        foreach (var element in items)
        {
            context.Variables[predicate.Parameters[0].Value!] = element;
            sum += Convert.ToDecimal(visitor.Visit(predicate, context).Value);
        }

        context.Variables.Remove(predicate.Parameters[0].Value!);
        return sum;
    }
}