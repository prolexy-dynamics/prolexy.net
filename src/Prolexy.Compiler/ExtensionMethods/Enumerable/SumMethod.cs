using System.Collections;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.Enumerable;

public record SumMethod : EnumerationExtensionMethod
{
    public SumMethod() : base("Sum", 
        Array.Empty<Parameter>(),
        PrimitiveType.Number)
    {
        MutableParameters = new List<Parameter>
        {
            new("selector",
                new MethodSignature(PrimitiveType.Void,
                    new Parameter[]
                    {
                        new("element", new GenericType("TElement"))
                    },
                    PrimitiveType.Number))
        };
    }

    public override object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context,
        object methodContext,
        IEnumerable<IAst> args)
    {
        if (methodContext is not IEnumerable items)
            throw new ArgumentException("Sum method can execute on Array types.");
        var arguments = args as IAst[] ?? args.ToArray();
        if (!arguments.Any())
            throw new ArgumentException("Selector not provided for 'Sum' method");
        if (arguments.First() is not AnonymousMethod predicate)
            throw new ArgumentException("Selector is not Anonymous method.");
        var sum = new decimal(0);
        var newScope = new Dictionary<string, object>();
        context.Variables.Push(newScope);
        foreach (var element in items)
        {
            newScope[predicate.Parameters[0].Value!] = element;
            sum += Convert.ToDecimal(visitor.Visit(predicate, context).Value);
        }

        context.Variables.Pop();
        return sum;
    }
}