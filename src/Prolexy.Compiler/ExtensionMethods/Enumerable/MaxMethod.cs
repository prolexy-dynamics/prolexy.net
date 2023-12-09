using System.Collections;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.Enumerable;

public record MaxMethod() : EnumerationExtensionMethod("Max",
    PrimitiveType.Boolean)
{
    public override object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context,
        object methodContext,
        IEnumerable<IAst> args)
    {
        if (methodContext is not IEnumerable items)
            throw new ArgumentException("Max method can execute on Array types.");
        var arguments = args as IAst[] ?? args.ToArray();
        if (!arguments.Any())
            throw new ArgumentException("Selector not provided for 'Max' method");
        if (arguments.First() is not AnonymousMethod predicate)
            throw new ArgumentException("Selector is not Anonymous method.");
        var bo = context.BusinessObject ?? JObject.Parse("{}");
        var max = decimal.MinValue;
        foreach (var element in items)
        {
            context.Variables[predicate.Parameters[0].Value!] = element;
            max = Math.Max(max, Convert.ToDecimal(visitor.Visit(predicate, context).Value));
        }

        context.Variables.Remove(predicate.Parameters[0].Value!);
        return max;
    }
}