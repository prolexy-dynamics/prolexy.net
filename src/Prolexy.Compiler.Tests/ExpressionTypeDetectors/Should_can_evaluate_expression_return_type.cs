using FluentAssertions;
using Prolexy.Compiler.ExtensionMethods.Enumerable;
using Prolexy.Compiler.Models;
using Prolexy.Compiler.Visitors.TypeDetectorVisitors;
using Tiba.Domain.Model.Uoms;

namespace Prolexy.Compiler.Tests.ExpressionTypeDetectors;

public class Should_can_evaluate_expression_return_type
{
    private TypeDetectorResult _result = null!;
    readonly ICompiler _compiler = new Implementations.Compiler();
    private string _expression;

    void GivenIEnterExpression(string expression)
    {
        _expression = expression;
    }

    void WhenIWantEvaluateReturnExpressionOnTheContext(object context)
    {
        var evalContext = EvaluatorContextBuilder
            .Default
            .AsClrEvaluatorBuilder()
            .AddClrType<MoneyData>()
            .AsExpressionTypeDetectorContextBuilder()
            .WithBusinessObjectType(context.GetType())
            .Build();
        var evaluator = _compiler.CompileExpression(_expression).AsExpressionClrReturnTypeEvaluator();
        _result = evaluator.Evaluate(evalContext);
    }

    void ThenIClrTypeAsAnExpected(Type expectedType)
    {
        _result.Result.Should().NotBeNull();
        var nullable = _result.Result!.Name.Contains("Nullable") && expectedType.Name.Contains("Nullable");
        if (!nullable)
            _result.Result.Should().Be(expectedType);
    }
}