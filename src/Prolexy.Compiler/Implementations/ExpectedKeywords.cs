using Prolexy.Compiler.Ast;

namespace Prolexy.Compiler.Implementations;

public class ExpectedKeywords : ParserException
{
    public ExpectedKeywords(TextSpan span,string[] keywords, int index):base(span, "Expected keywords")
    {
    }
}