using System.Collections;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.Enumerable;

public record LastMethod() : EnumerationExtensionMethod("Last",
    Array.Empty<Parameter>(),
    new GenericType("TElement"))
{
    public override object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context,
        object methodContext,
        IEnumerable<IAst> args)
    {
        if (methodContext is not IEnumerable items)
            throw new ArgumentException("Last method can execute on Array types.");

        object? result = items.Cast<object?>().LastOrDefault();

        if (result == null)
            throw new Exception("No any element at this list.");
        return result;
    }
}