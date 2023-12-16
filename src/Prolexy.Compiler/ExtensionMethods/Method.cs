using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods;

public abstract record Method(string Name, IType ReturnType) : IMethod, IType
{
    public abstract object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context,
        object methodContext,
        IEnumerable<IAst> args);

    public IType? GetPropertyType(string name)
    {
        return ReturnType;
    }

    public abstract bool Accept(object value);
}

public interface IMethod
{
    object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context,
        object methodContext, IEnumerable<IAst> args);
}

public record ClrMethodMethod(string Name, IType ReturnType) : IMethod
{
    public object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context,
        object methodContext, IEnumerable<IAst> args)
    {
        throw new NotImplementedException();
    }
}