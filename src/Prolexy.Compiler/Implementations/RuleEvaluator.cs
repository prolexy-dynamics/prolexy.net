using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.Implementations;

class RuleEvaluator<T> : IRuleEvaluator<T> where T : JToken
{
    private readonly IAst _ast;
    private readonly IAstVisitor<EvaluatorContext, EvaluatorResult> _visitor;

    public RuleEvaluator(IAst ast, IAstVisitor<EvaluatorContext, EvaluatorResult> visitor)
    {
        _ast = ast;
        _visitor = visitor;
    }

    public T? Evaluate(EvaluatorContext evaluatorContext)
    {
        return (T?)_ast.Visit(_visitor, evaluatorContext).Value;
    }
}
class RuleEvaluator : IRuleEvaluator
{
    private readonly IAst _ast;
    private readonly IAstVisitor<EvaluatorContext, EvaluatorResult> _visitor;

    public RuleEvaluator(IAst ast, IAstVisitor<EvaluatorContext, EvaluatorResult> visitor)
    {
        _ast = ast;
        _visitor = visitor;
    }

    public void Execute(EvaluatorContext evaluatorContext)
    {
        _ast.Visit(_visitor, evaluatorContext);
    }
}