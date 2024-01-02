using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Models;
using Prolexy.Compiler.SchemaGenerators;
using Tiba.Domain.Model.Uoms;

namespace Prolexy.Compiler.ExtensionMethods.StringExtensions;

public abstract record StringExtensionMethod(string Name, IEnumerable<Parameter> Parameters, IType ReturnType) :
    Method(Name, PrimitiveType.String, Parameters, ReturnType)
{
    public override bool Accept(object value)
    {
        return value is JToken { Type: JTokenType.String } or String;
    }
}
public record MoneyMethod() :
    Method("Add", 
        new ClrType<Money>(), 
        new Parameter[]{ new ("money", new ClrType<Money>())},
        new ClrType<Money>())
{
    public override object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context, object methodContext, IEnumerable<IAst> args)
    {
        throw new NotImplementedException();
    }

    public override bool Accept(object value)
    {
        return value is JToken { Type: JTokenType.String } or String;
    }
}