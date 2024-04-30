using System.Collections;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.StringExtensions;

public record SplitByMethod() : StringExtensionMethod("SplitBy",
    new Parameter[]
    {
        new("text", PrimitiveType.String)
    },
    PrimitiveType.String)
{
    public override object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context,
        object methodContext,
        IEnumerable<IAst> args)
    {
        if (methodContext is not String text)
            throw new ArgumentException("SplitBy method can execute on string types.");
        var firstArgValue = visitor.Visit(args.First(), context).Value;
        if (firstArgValue is not string splitBy)
            throw new ArgumentException("SplitBy accept string parameter.");

        object? result = text.Split(splitBy, StringSplitOptions.RemoveEmptyEntries);

        return result;
    }
}