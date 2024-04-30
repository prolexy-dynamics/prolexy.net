using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.ExtensionMethods;

public interface IMethod : IType
{
    object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context,
        object methodContext, IEnumerable<IAst> args);
}