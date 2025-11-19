using FluentAssertions;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Implementations.Visitors;
using Prolexy.Compiler.Implementations.Visitors.Formatters;
using Prolexy.Compiler.Models;
using Prolexy.Compiler.Tests.LogicalSchemaGenerators;

namespace Prolexy.Compiler.Tests.SchemaFormatters;

public abstract class SchemaFormatterTestBase
{
    private readonly LogicalSchemaVisitor _visitor = new();

    protected abstract string Expression { get; }
    protected abstract JObject ExpectedJsonSchema { get; }
    protected abstract JObject ExpectedFlatSchema { get; }

    private JObject RunJsonFormatter()
    {
        var ir = Parse(Expression);
        return new JsonSchemaFormatter().Format(ir);
    }

    private JObject RunFlatFormatter()
    {
        var ir = Parse(Expression);
        return new FlatSchemaFormatter().Format(ir);
    }

    private LogicalSchemaNode Parse(string expr)
    {
        var parser = new Parser();
        var ast = parser.ParseExpression(expr);

        var context = EvaluatorContextBuilder
            .Default.AsClrEvaluatorBuilder()
            .WithBusinessObject(new LogicalSchemaVisitorTests.MyBusinessObject())
            .Build();

        return ast.Visit(_visitor, context);
    }

    [Fact]
    public void JsonSchema_ShouldMatchExpected()
    {
        var actual = RunJsonFormatter();
        actual.Should().BeEquivalentTo(ExpectedJsonSchema);
    }

    [Fact]
    public void FlatSchema_ShouldMatchExpected()
    {
        var actual = RunFlatFormatter();
        actual.Should().BeEquivalentTo(ExpectedFlatSchema);
    }
}