using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.Enumerable;

public record MaxMethod() : EnumerationExtensionMethod("Max",
    PrimitiveType.Boolean)
{
    public override JToken? Eval(EvaluatorVisitor visitor, EvaluatorContext context, JToken methodContext,
        IEnumerable<IAst> args)
    {
        if (methodContext.Type != JTokenType.Array)
            throw new ArgumentException("Max method can execute on Array types.");
        var arguments = args as IAst[] ?? args.ToArray();
        if (!arguments.Any())
            throw new ArgumentException("Selector not provided for 'Max' method");
        if (arguments.First() is not AnonymousMethod predicate)
            throw new ArgumentException("Selector is not Anonymous method.");
        var bo = context.BusinessObject ?? JObject.Parse("{}");
        var oldValue = bo[predicate.Parameters[0].Value!];
        var max = decimal.MinValue;
        foreach (var element in methodContext)
        {
            bo[predicate.Parameters[0].Value!] = element;
            max = Math.Max(max,
                Convert.ToDecimal(predicate.Visit(visitor, context with { BusinessObject = bo }).Value));
        }

        bo[predicate.Parameters[0].Value!] = oldValue;
        return max;
    }
}