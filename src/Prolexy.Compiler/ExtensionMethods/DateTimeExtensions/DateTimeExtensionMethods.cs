using System.Globalization;
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

public record FormatDatetimeExtensions() : Method("Format", PrimitiveType.Datetime,
    new[] { new Parameter("format", PrimitiveType.String) },
    PrimitiveType.Boolean)
{
    public override object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context,
        object methodContext, IEnumerable<IAst> args)
    {
        var format = visitor.Visit(args.First(), context).Value.ToString();
        var datetime = Convert.ToDateTime(methodContext);
        return datetime.ToLocalTime().ToString(format, CultureInfo.GetCultureInfo("fa-Ir"));
    }

    public override bool Accept(object value, bool implicitAccessMethod)
    {
        return value is JToken { Type: JTokenType.Date } or DateTime;
    }
}

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