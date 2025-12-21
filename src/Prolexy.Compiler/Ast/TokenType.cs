namespace Prolexy.Compiler.Ast;

public enum TokenType
{
    Keyword = 1,
    Operation = 2,
    Const = 3,
    Identifier = 4,
    Invalid = 8,
    Eof = 16,
}