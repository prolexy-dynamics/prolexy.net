using System.Collections;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.Enumerable;

public record CountMethod() : EnumerationExtensionMethod("Count",
    new Parameter[]
    {
        new("predicate",
            new MethodSignature(new Parameter[]
                {
                    new("element", new GenericType("T"))
                },
                PrimitiveType.Boolean))
    },
    PrimitiveType.Number)
{
    public override object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context,
        object methodContext,
        IEnumerable<IAst> args)
    {
        if (methodContext is not IEnumerable items)
            throw new ArgumentException("Count method can execute on Array types.");
        var arguments = args as IAst[] ?? args.ToArray();
        if (!arguments.Any())
            throw new ArgumentException("Predicate not provided for 'Count' method");
        if (arguments.First() is not AnonymousMethod predicate)
            throw new ArgumentException("predicate is not Anonymous method.");
        var count = 0;
        foreach (var element in items)
        {
            context.Variables[predicate.Parameters[0].Value!] = element;

            if (Convert.ToBoolean(visitor.Visit(predicate, context).Value)) count++;
        }
        context.Variables.Remove(predicate.Parameters[0].Value!);

        return count;
    }
}