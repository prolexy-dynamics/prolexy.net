using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.Implementations;

class RuleEvaluator<TC, TR> : IRuleEvaluator<TC, TR>  where TC : IEvaluatorContext where TR : IEvaluatorResult
{
    private readonly IAst _ast;
    private readonly IEvaluatorVisitor _visitor;

    public RuleEvaluator(IAst ast, IEvaluatorVisitor visitor)
    {
        _ast = ast;
        _visitor = visitor;
    }

    public TR? Evaluate(TC evaluatorContext)
    {
        return (TR?)_visitor.Visit(_ast, evaluatorContext);
    }
}
// class RuleEvaluator : IRuleEvaluator<EvaluatorContext>
// {
//     private readonly IAst _ast;
//     private readonly IEvaluatorVisitor<EvaluatorContext, EvaluatorResult> _visitor;
//
//     public RuleEvaluator(IAst ast, IEvaluatorVisitor<EvaluatorContext, EvaluatorResult> visitor)
//     {
//         _ast = ast;
//         _visitor = visitor;
//     }
//
//     public void Execute(EvaluatorContext evaluatorContext)
//     {
//         _visitor.Visit(_ast, evaluatorContext);
//     }
// }