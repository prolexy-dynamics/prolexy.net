using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.DateTimeExtensions;

public record NowMethod() : Method("Now", PrimitiveType.Datetime, Array.Empty<Parameter>(),
    PrimitiveType.Boolean)
{
    public override object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context,
        object methodContext, IEnumerable<IAst> args)
    {
        return DateTime.Now;
    }

    public override bool Accept(object value, bool implicitAccessMethod)
    {
        return implicitAccessMethod;
    }
}