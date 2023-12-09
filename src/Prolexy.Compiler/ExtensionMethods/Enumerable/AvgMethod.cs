using System.Collections;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.Enumerable;

public record AvgMethod() : EnumerationExtensionMethod("Avg",
    PrimitiveType.Boolean)
{
    public override object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context,
        object methodContext,
        IEnumerable<IAst> args)
    {
        if (methodContext is not IEnumerable items)
            throw new ArgumentException("Avg method can execute on Array types.");
        var arguments = args as IAst[] ?? args.ToArray();
        if (!arguments.Any())
            throw new ArgumentException("Selector not provided for 'Avg' method");
        if (arguments.First() is not AnonymousMethod predicate)
            throw new ArgumentException("Selector is not Anonymous method.");
        var sum = new decimal(0);
        var count = 0;
        foreach (var element in items)
        {
            context.Variables[predicate.Parameters[0].Value!] = element;
        
            sum += Convert.ToDecimal(visitor.Visit(predicate, context).Value);
            count++;
        }
        context.Variables.Remove(predicate.Parameters[0].Value!);

        return count == 0 ? 0 : sum / count;
    }
}

// public record GroupByMethod() : EnumerationExtensionMethod("GroupBy",
//     PrimitiveType.Boolean)
// {
//     public override JToken? Eval(IAstVisitor visitor, IEvaluatorContext context,
//         object methodContext,
//         IEnumerable<IAst> args)
//     {
//         var arguments = args as IAst[] ?? args.ToArray();
//         if (!arguments.Any())
//             throw new ArgumentException("Key Selector not provided for 'Avg' method");
//         if (!arguments.Skip(1).Any())
//             throw new ArgumentException("Value Selector not provided for 'Avg' method");
//         if (arguments.First() is not AnonymousMethod keySelector)
//             throw new ArgumentException("Key Selector is not Anonymous method.");
//         if (arguments.Skip(1).First() is not AnonymousMethod valueSelector)
//             throw new ArgumentException("Value Selector is not Anonymous method.");
//
//         var result = JObject.Parse("{}");
//         foreach (var element in methodContext)
//         {
//             bo[keySelector.Parameters[0].Value!] = element;
//             var key = (string)keySelector.Visit(visitor, context with { BusinessObject = bo }).Value!;
//             var values = (JArray)(result[key] ??= new JArray());
//             values.Add(valueSelector.Visit(visitor, context with { BusinessObject = bo }).Value!);
//         }
//
//         return result;
//     }
// }