using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.StringExtensions;

public record IffMethod() : Method("Iff",
    PrimitiveType.Void, 
    new Parameter[]
    {
        new("text", PrimitiveType.Boolean),
        new("trueValue", new GenericType("T")),
        new("falseValue", new GenericType("T")),
    },
    new GenericType("T"))
{
    public override object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context,
        object methodContext,
        IEnumerable<IAst> args)
    {
        var parameters = args.ToArray();
        var condition = visitor.Visit(parameters[0], context).Value;
        if (condition is not bool satisfied)
            throw new ArgumentException("SplitBy accept string parameter.");
        return satisfied ? visitor.Visit(parameters[1], context).Value : visitor.Visit(parameters[2], context).Value;
    }

    public override bool Accept(object value, bool implicitAccessMethod)
    {
        return true;
    }
}