using System.Collections;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.Enumerable;

public record ExistsMethod : EnumerationExtensionMethod
{
    public ExistsMethod() : base("Exists",
        new Parameter[]{},
        PrimitiveType.Boolean)
    {
        MutableParameters = new List<Parameter>
        {
            new("predicate",
                new MethodSignature(PrimitiveType.Void, new Parameter[]
                    {
                        new("element", new GenericType("TElement"))
                    },
                    PrimitiveType.Boolean))
        };
    }

    public override object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context,
        object methodContext,
        IEnumerable<IAst> args)
    {
        if (methodContext is not IEnumerable items)
            throw new ArgumentException("Any method can execute on Array types.");
        var arguments = args as IAst[] ?? args.ToArray();
        if (!arguments.Any())
            throw new ArgumentException("Predicate not provided for 'Any' method");
        if (arguments.First() is not AnonymousMethod predicate)
            throw new ArgumentException("predicate is not Anonymous method.");

        foreach (var element in items)
        {
            context.Variables[predicate.Parameters[0].Value!] = element;
            if (Convert.ToBoolean(visitor.Visit(predicate, context).Value))
            {
                context.Variables.Remove(predicate.Parameters[0].Value!);
                return true;
            }
        }

        context.Variables.Remove(predicate.Parameters[0].Value!);
        return false;
    }
}