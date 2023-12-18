using FluentAssertions;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.ExtensionMethods;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.Tests.Evaluator;

public class Should_can_evaluate_expression_on_Clr_context
{
    private IRuleEvaluator<ClrEvaluatorContext, ClrEvaluatorResult> _evaluator = null!;
    private object? _result;

    void GivenICompileInput(string expression)
    {
        var compiler = new Implementations.Compiler();
        _evaluator = compiler.CompileExpression(expression).AsClrContext();
    }

    void WhenEvaluateCompiledCode(object context)
    {
        var ctx = ClrEvaluatorContextBuilder.Default
            .WithBusinessObject(context)
            .AddClrType<Person>()
            .Build();
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

public class Person
{
    public Person(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
