using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler;

public interface ICompiler
{
    ICompiledSource Compile(string source);
    ICompiledSource CompileExpression(string source);
}

public interface ICompiledSource
{
    IRuleEvaluator<EvaluatorContext, EvaluatorResult> AsJsonContext();
    IRuleEvaluator<ClrEvaluatorContext, ClrEvaluatorResult> AsClrContext();
}

class CompiledSource : ICompiledSource
{
    private readonly IAst _ast;

    public CompiledSource(IAst ast)
    {
        _ast = ast;
    }

    public IRuleEvaluator<EvaluatorContext, EvaluatorResult> AsJsonContext()
    {
        return new RuleEvaluator<EvaluatorContext, EvaluatorResult>(_ast, new EvaluatorVisitor());
    }
    public IRuleEvaluator<ClrEvaluatorContext, ClrEvaluatorResult> AsClrContext()
    {
        return new RuleEvaluator<ClrEvaluatorContext, ClrEvaluatorResult>(_ast, new ClrEvaluatorVisitor());
    }

}