using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.Enumerable;

public record ExistsMethod() : EnumerationExtensionMethod("Exists",
    PrimitiveType.Boolean)
{
    public override JToken? Eval(EvaluatorVisitor visitor, EvaluatorContext context, JToken methodContext,
        IEnumerable<IAst> args)
    {
        if (methodContext.Type != JTokenType.Array)
            throw new ArgumentException("Any method can execute on Array types.");
        var arguments = args as IAst[] ?? args.ToArray();
        if (!arguments.Any())
            throw new ArgumentException("Predicate not provided for 'Any' method");
        if (arguments.First() is not AnonymousMethod predicate)
            throw new ArgumentException("predicate is not Anonymous method.");
        var bo = context.BusinessObject ?? JObject.Parse("{}");
        var oldValue = bo[predicate.Parameters[0].Value!];
        foreach (var element in methodContext)
        {
            bo[predicate.Parameters[0].Value!] = element;
            if (Convert.ToBoolean(predicate.Visit(visitor, context with { BusinessObject = bo }).Value))
            {
                bo[predicate.Parameters[0].Value!] = oldValue;
                return true;
            }
        }

        bo[predicate.Parameters[0].Value!] = oldValue;
        return false;
    }
}