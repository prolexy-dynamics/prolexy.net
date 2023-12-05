using FluentAssertions;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.Tests.Evaluator;

public class Should_can_execute
{
    private object? _result;
    readonly ICompiler _compiler = new Implementations.Compiler();
    private EvaluatorContext? _evalContext;

    void GivenIExecuteExpression(string rule, string context)
    {
        var evaluator = _compiler.Compile(rule);
        _evalContext = EvaluatorContextBuilder.Default.WithBusinessObject(JObject.Parse(context)).Build();
        evaluator.Execute(_evalContext);
    }

    void WhenIEvaluateExpression(string trueExpression)
    {
        var result = _compiler.CompileAsExpression<JToken>(trueExpression).Evaluate(_evalContext!);
        _result = (bool?)result;
    }

    void ThenISeeCSharpLikeSyntaxAsAnExpected()
    {
        _result.Should().Be(true);
    }
}