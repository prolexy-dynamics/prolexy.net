using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.DateTimeExtensions;

public record TomorrowMethod() : Method("Tomorrow", PrimitiveType.Datetime, Array.Empty<Parameter>(),
    PrimitiveType.Boolean)
{
    public override object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context,
        object methodContext, IEnumerable<IAst> args)
    {
        return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1);
    }

    public override bool Accept(object value, bool implicitAccessMethod)
    {
        return implicitAccessMethod;
    }
}