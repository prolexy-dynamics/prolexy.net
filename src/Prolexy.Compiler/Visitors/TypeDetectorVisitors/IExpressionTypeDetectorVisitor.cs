using Prolexy.Compiler.Ast;

namespace Prolexy.Compiler.Visitors.TypeDetectorVisitors;

public interface IExpressionTypeDetectorVisitor : IAstVisitor<ExpressionTypeDetectorContext, TypeDetectorResult>
{
    TypeDetectorResult Visit(IAst ast, ExpressionTypeDetectorContext context);
}