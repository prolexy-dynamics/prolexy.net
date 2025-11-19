using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler;

public interface IEvaluatorVisitor
{
    IEvaluatorResult Visit(IAst ast, IEvaluatorContext context);
}