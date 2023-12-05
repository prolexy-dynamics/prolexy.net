using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.Enumerable;

public record CountMethod() : EnumerationExtensionMethod("Count",
    PrimitiveType.Boolean)
{
    public override JToken? Eval(EvaluatorVisitor visitor, EvaluatorContext context, JToken methodContext,
        IEnumerable<IAst> args)
    {
        if (methodContext.Type != JTokenType.Array)
            throw new ArgumentException("Count method can execute on Array types.");
        var arguments = args as IAst[] ?? args.ToArray();
        if (!arguments.Any())
            throw new ArgumentException("Predicate not provided for 'Count' method");
        if (arguments.First() is not AnonymousMethod predicate)
            throw new ArgumentException("predicate is not Anonymous method.");
        var bo = context.BusinessObject ?? JObject.Parse("{}");
        var oldValue = bo[predicate.Parameters[0].Value!];
        var count = 0;
        foreach (var element in methodContext)
        {
            bo[predicate.Parameters[0].Value!] = element;
            if (Convert.ToBoolean(predicate.Visit(visitor, context with { BusinessObject = bo }).Value)) count++;
        }

        bo[predicate.Parameters[0].Value!] = oldValue;
        return count;
    }
}