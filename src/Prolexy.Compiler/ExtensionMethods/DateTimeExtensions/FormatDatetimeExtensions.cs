using System.Globalization;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.DateTimeExtensions;

public record FormatDatetimeExtensions() : Method("Format", PrimitiveType.Datetime,
    new[]
    {
        new Parameter("format", PrimitiveType.String),
        new Parameter("culture", PrimitiveType.String)
    },
    PrimitiveType.String)
{
    public override object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context,
        object methodContext, IEnumerable<IAst> args)
    {
        var enumerable = args as IAst[] ?? args.ToArray();
        var format = visitor.Visit(enumerable.First(), context).Value.ToString();
        var datetime = Convert.ToDateTime(methodContext);
        var culture = CultureInfo.GetCultureInfo(enumerable.Count() > 1 ? visitor.Visit(enumerable.ElementAt(1), context).Value.ToString() : "fa-Ir");
        return datetime.ToString(format, culture);
    }

    public override bool Accept(object value, bool implicitAccessMethod)
    {
        return value is JToken { Type: JTokenType.Date } or DateTime;
    }
}