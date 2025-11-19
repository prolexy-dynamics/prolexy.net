using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Implementations.Visitors;
using Prolexy.Compiler.Models;
using Prolexy.Compiler.Visitors.TypeDetectorVisitors;

namespace Prolexy.Compiler;

public interface ICompiledSource
{
    IRuleEvaluator<EvaluatorContext, EvaluatorResult> AsJsonEvaluator();
    IRuleEvaluator<ClrEvaluatorContext, ClrEvaluatorResult> AsClrEvaluator();
    IRuleEvaluator<ClrEvaluatorContext, LogicalSchemaNode> AsLogicalSchemaGenerator();
    IRuleEvaluator<ExpressionTypeDetectorContext, TypeDetectorResult> AsExpressionClrReturnTypeEvaluator();
}