using FluentAssertions;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Implementations;

namespace Prolexy.Compiler.Tests.ParserTests;

public class Should_can_parse_statements
{
    private IAst ast = null!;

    void WhenParseStatements(string input)
    {
        ast = new Parser().Parse(input);
    }

    void ThenISeeCSharpLikeSyntaxAsAnExpected(string expectedSyntax)
    {
        var output = (string?)ast.Visit(new TranslateAstVisitor(), new FormatterContext(0));
        output?.Replace(" ", "").Replace("\n", "")
            .Should().Be(expectedSyntax.Replace(" ", "").Replace("\n", ""));
    }
}