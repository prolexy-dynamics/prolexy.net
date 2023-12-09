using FluentAssertions;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.ExtensionMethods;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.Tests.Evaluator;

public class Should_can_evaluate_expression_on_Json_constext
{
    private IRuleEvaluator<object> _evaluator = null!;
    private object? _result;

    void GivenICompileInput(string expression)
    {
        var compiler = new Implementations.Compiler();
        _evaluator = compiler.CompileAsExpression<JToken>(expression);
    }

    void WhenEvaluateCompiledCode(string context)
    {
        var ctx = EvaluatorContextBuilder.Default
            .WithSchema(new Schema(new[] { new Property("OrderDate", PrimitiveType.Datetime) }, new Method[] { }))
            .WithBusinessObject(JObject.Parse(context)).Build();
        _result = _evaluator.Evaluate(ctx);
    }

    void ThenISeeCSharpLikeSyntaxAsAnExpected(object? expected)
    {
        if (expected is null)
            _result.Should().BeNull();
        else
            Convert.ChangeType(_result, expected.GetType()).Should().Be(expected);
    }
}
public class Should_can_evaluate_expression_on_Json_constext
{
    private IRuleEvaluator<object> _evaluator = null!;
    private object? _result;

    void GivenICompileInput(string expression)
    {
        var compiler = new Implementations.Compiler();
        _evaluator = compiler.CompileAsExpression<JToken>(expression);
    }

    void WhenEvaluateCompiledCode(string context)
    {
        var ctx = EvaluatorContextBuilder.Default
            .WithSchema(new Schema(new[] { new Property("OrderDate", PrimitiveType.Datetime) }, new Method[] { }))
            .WithBusinessObject(JObject.Parse(context)).Build();
        _result = _evaluator.Evaluate(ctx);
    }

    void ThenISeeCSharpLikeSyntaxAsAnExpected(object? expected)
    {
        if (expected is null)
            _result.Should().BeNull();
        else
            Convert.ChangeType(_result, expected.GetType()).Should().Be(expected);
    }
}