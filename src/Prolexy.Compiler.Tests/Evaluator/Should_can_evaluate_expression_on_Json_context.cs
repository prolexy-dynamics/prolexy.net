using FluentAssertions;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.ExtensionMethods;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.Tests.Evaluator;

public class Should_can_evaluate_expression_on_Json_context
{
    private IRuleEvaluator<EvaluatorContext, EvaluatorResult> _evaluator = null!;
    private object? _result;

    void GivenICompileInput(string expression)
    {
        var compiler = new Implementations.Compiler();
        _evaluator = compiler.CompileExpression(expression).AsJsonContext();
    }

    void WhenEvaluateCompiledCode(string context)
    {
        var ctx = EvaluatorContextBuilder.Default
            .WithSchema(new Schema("Order", 
                new[] { new Property("OrderDate", PrimitiveType.Datetime) }, 
                new Method[] { },
                new Method[] { }))
            .WithBusinessObject(JObject.Parse(context)).Build();
        _result = _evaluator.Evaluate(ctx)?.Value;
    }

    void ThenISeeCSharpLikeSyntaxAsAnExpected(object? expected)
    {
        if (expected is null)
            _result.Should().BeNull();
        else
            Convert.ChangeType(_result, expected.GetType()).Should().Be(expected);
    }
}