using System.Collections;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.Enumerable;

public record MinMethod() : EnumerationExtensionMethod("Min", new Parameter[]
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
            throw new ArgumentException("Min method can execute on Array types.");
        var arguments = args as IAst[] ?? args.ToArray();
        if (!arguments.Any())
            throw new ArgumentException("Selector not provided for 'Min' method");
        if (arguments.First() is not AnonymousMethod predicate)
            throw new ArgumentException("Selector is not Anonymous method.");

        var min = decimal.MaxValue;
        foreach (var element in items)
        {
            context.Variables[predicate.Parameters[0].Value!] = element;
            min = Math.Min(min, Convert.ToDecimal(visitor.Visit(predicate, context).Value));
        }

        context.Variables.Remove(predicate.Parameters[0].Value!);
        return min;
    }
}