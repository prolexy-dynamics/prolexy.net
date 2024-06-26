using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.DateTimeExtensions;

public record AddDaysMethod() : Method("AddDays", PrimitiveType.Datetime,
    new[] { new Parameter("days", PrimitiveType.Number) },
    PrimitiveType.Boolean)
{
    public override object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context,
        object methodContext, IEnumerable<IAst> args)
    {
        var days = Convert.ToInt32(visitor.Visit(args.First(), context).Value);
        return Convert.ToDateTime(methodContext).AddDays(days);
    }

    public override bool Accept(object value, bool implicitAccessMethod)
    {
        return value is JToken { Type: JTokenType.Date } or DateTime;
    }
}