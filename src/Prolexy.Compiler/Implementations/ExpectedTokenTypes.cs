using Prolexy.Compiler.Ast;

namespace Prolexy.Compiler.Implementations;

public class ParserException(TextSpan CurrentSpan, string message) : Exception(message)
{
    public TextSpan CurrentSpan { get; } = CurrentSpan;
}

public class ExpectedTokenTypes : ParserException
{
    public ExpectedTokenTypes(TextSpan span, TokenType[] p0, string[]? strings) :base(span, "Expected token types")
    {
    }
}