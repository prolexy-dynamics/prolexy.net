using Prolexy.Compiler.Ast;

namespace Prolexy.Compiler;

public interface IParser
{
    IAst Parse(Token[] tokens);
    IAst Parse(string source);
    IAst ParseExpression(Token[] tokens);
    IAst ParseExpression(string source);
}