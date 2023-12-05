using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;

namespace Prolexy.Compiler;

public interface ICompiler
{
    IRuleEvaluator Compile(Token[] tokens);
    IRuleEvaluator Compile(string source);
    IRuleEvaluator<T> CompileAsExpression<T>(Token[] tokens) where T : JToken;
    IRuleEvaluator<T> CompileAsExpression<T>(string source) where T : JToken;
}