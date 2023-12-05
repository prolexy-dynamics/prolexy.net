using FluentAssertions;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Implementations;

namespace Prolexy.Compiler.Tests.ParserTests;

public class Should_can_parse_expression
{
    private IAst ast = null!;

    void WhenParseAssignment(string input)
    {
        ast = new Parser().ParseExpression(input);
    }

    void ThenISeeCSharpLikeSyntaxAsAnExpected(string expectedSyntax)
    {
        var output = ast.Visit(new TranslateAstVisitor(), new FormatterContext(0))?.ToString();
        output?.Replace(" ", "").Replace("\n", "")
            .Should().Be(expectedSyntax.Replace(" ", "").Replace("\n", ""));
    }
}