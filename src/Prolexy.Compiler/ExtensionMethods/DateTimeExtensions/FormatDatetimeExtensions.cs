using System.Globalization;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.DateTimeExtensions;

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