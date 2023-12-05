using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.DateTimeExtensions;

public record AddDaysMethod() : Method("AddDays",
    PrimitiveType.Boolean)
{
    public override JToken? Eval(EvaluatorVisitor visitor, EvaluatorContext context, JToken methodContext, IEnumerable<IAst> args)
    {
        var days = Convert.ToInt32(args.First().Visit(visitor, context).Value);
        return Convert.ToDateTime(methodContext).AddDays(days);

    }

    public override bool Accept(JToken? value)
    {
        return value is { Type: JTokenType.Date };
    }
}
public record NowMethod() : Method("Now",
    PrimitiveType.Boolean)
{
    public override JToken? Eval(EvaluatorVisitor visitor, EvaluatorContext context, JToken methodContext, IEnumerable<IAst> args)
    {
        return DateTime.Now;
    }

    public override bool Accept(JToken? value)
    {
        return value == null;
    }
}
