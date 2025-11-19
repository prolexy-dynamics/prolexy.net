using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Implementations.Visitors;
using Prolexy.Compiler.Models;
using Prolexy.Compiler.Visitors.TypeDetectorVisitors;

namespace Prolexy.Compiler;

class CompiledSource : ICompiledSource
{
    private readonly IAst _ast;

    public CompiledSource(IAst ast)
    {
        _ast = ast;
    }

    public IRuleEvaluator<EvaluatorContext, EvaluatorResult> AsJsonEvaluator()
    {
        return new RuleEvaluator<EvaluatorContext, EvaluatorResult>(_ast, new EvaluatorVisitor());
    }
    public IRuleEvaluator<ClrEvaluatorContext, ClrEvaluatorResult> AsClrEvaluator()
    {
        return new RuleEvaluator<ClrEvaluatorContext, ClrEvaluatorResult>(_ast, new ClrEvaluatorVisitor());
    }

    public IRuleEvaluator<ClrEvaluatorContext, LogicalSchemaNode> AsLogicalSchemaGenerator()
    {
        return new RuleEvaluator<ClrEvaluatorContext, LogicalSchemaNode>(_ast, new LogicalSchemaVisitor());
    }

    public IRuleEvaluator<ExpressionTypeDetectorContext, TypeDetectorResult> AsExpressionClrReturnTypeEvaluator()
    {
        return new RuleEvaluator<ExpressionTypeDetectorContext, TypeDetectorResult>(_ast, new ClrExpressionTypeDetectorVisitor());
    }
}