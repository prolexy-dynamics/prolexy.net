using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods.Utilities;

public record EvalMethod() : Method("Eval", PrimitiveType.Void, new[]
    {
        new Parameter("script", PrimitiveType.String)
    },
    Dynamic.Instance)
{
    public override object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context, object methodContext,
        IEnumerable<IAst> args)
    {
        var parser = new Parser();
        var script = parser.ParseExpression(visitor.Visit(args.First(), context).Value.ToString());
        return visitor.Visit(script, context).Value;
    }

    public override bool Accept(object value, bool implicitAccessMethod)
    {
        return implicitAccessMethod;
    }
}