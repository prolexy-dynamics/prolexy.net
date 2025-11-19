using Newtonsoft.Json.Linq;

namespace Prolexy.Compiler;

public interface ICompiler
{
    ICompiledSource Compile(string source);
    ICompiledSource CompileExpression(string source);
}