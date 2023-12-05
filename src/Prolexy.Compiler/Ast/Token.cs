namespace Prolexy.Compiler.Ast;

public record Token(TokenType TokenType, string? Value = null, string? Type = null)
{
    public static Token Eof { get; } = new Token(TokenType.Eof);
}