using Prolexy.Compiler.Ast;

namespace Prolexy.Compiler.Implementations;

internal class ExpectedTokenTypes : Exception
{
    public ExpectedTokenTypes(TokenType[] p0, string[]? strings)
    {
    }
}