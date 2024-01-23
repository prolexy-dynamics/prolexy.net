using System.Collections;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.Enumerable;

public record CountMethod : EnumerationExtensionMethod
{
    public CountMethod() : base("Count",
        new Parameter[0],
        PrimitiveType.Number)
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
            throw new ArgumentException("Count method can execute on Array types.");
        var arguments = args as IAst[] ?? args.ToArray();
        if (!arguments.Any())
            throw new ArgumentException("Predicate not provided for 'Count' method");
        if (arguments.First() is not AnonymousMethod predicate)
            throw new ArgumentException("predicate is not Anonymous method.");
        var count = 0;
        var newScope = new Dictionary<string, object>();
        context.Variables.Push(newScope);
        foreach (var element in items)
        {
            newScope[predicate.Parameters[0].Value!] = element;

            if (Convert.ToBoolean(visitor.Visit(predicate, context).Value)) count++;
        }
        context.Variables.Pop();

        return count;
    }
}