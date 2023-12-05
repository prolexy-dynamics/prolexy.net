using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods;

public abstract record Method(string Name, IType ReturnType) : IType
{
    public abstract JToken? Eval(EvaluatorVisitor visitor, EvaluatorContext context, JToken methodContext,
        IEnumerable<IAst> args);

    public IType? GetSubType(string name)
    {
        return ReturnType;
    }

    public abstract bool Accept(JToken? value);
}