using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;

namespace Prolexy.Compiler.Implementations;

public class Compiler : ICompiler
{
    private readonly IParser _parser = new Parser();
    private EvaluatorVisitor _visitor = new();

    public ICompiledSource Compile(string input)
    {
        var ast = _parser.Parse(input);
        return new CompiledSource(ast);
    }

    public ICompiledSource CompileExpression(string source)
    {
        var ast = _parser.ParseExpression(source);
        return new CompiledSource(ast);
    }
}