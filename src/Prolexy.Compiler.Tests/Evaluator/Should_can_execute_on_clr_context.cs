using FluentAssertions;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.Tests.Evaluator;

public class Should_can_execute_on_clr_context
{
    private object? _result;
    readonly ICompiler _compiler = new Implementations.Compiler();
    private ClrEvaluatorContext? _evalContext;

    void GivenIExecuteExpression(string rule, object context)
    {
        var evaluator = _compiler.Compile(rule).AsClrEvaluator();
        _evalContext = EvaluatorContextBuilder
            .Default
            .AsClrEvaluatorBuilder()
            .WithBusinessObject(context)
            .Build();
        evaluator.Evaluate(_evalContext);
    }

    void WhenIEvaluateExpression(string trueExpression)
    {
        var result = _compiler.CompileExpression(trueExpression).AsClrEvaluator().Evaluate(_evalContext!)?.Value;
        _result = (bool?)result;
    }

    void ThenISeeCSharpLikeSyntaxAsAnExpected()
    {
        _result.Should().Be(true);
    }
}