using System.Collections;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.Enumerable;

public record SingleMethod() : EnumerationExtensionMethod("Single",
    new Parameter[]
    {
        new("predicate",
            new MethodSignature(new Parameter[]
                {
                    new("element", new GenericType("T"))
                },
                PrimitiveType.Boolean))
    },
    new GenericType("T"))
{
    public override object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context,
        object methodContext,
        IEnumerable<IAst> args)
    {
        if (methodContext is not IEnumerable items)
            throw new ArgumentException("Single method can execute on Array types.");
        var arguments = args as IAst[] ?? args.ToArray();
        if (!arguments.Any())
            throw new ArgumentException("Selector not provided for 'Single' method");
        if (arguments.First() is not AnonymousMethod predicate)
            throw new ArgumentException("Selector is not Anonymous method.");

        object result = null;
        foreach (var element in items)
        {
            context.Variables[predicate.Parameters[0].Value!] = element;
            var condition = (bool)visitor.Visit(predicate, context).Value;
            if (condition)
            {
                result = element;
                break;
            }
        }

        if (result == null)
            throw new Exception("No any element matched.");
        context.Variables.Remove(predicate.Parameters[0].Value!);
        return result;
    }
}