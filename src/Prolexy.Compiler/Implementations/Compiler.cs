using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;

namespace Prolexy.Compiler.Implementations;

public class Compiler : ICompiler
{
    private readonly IParser _parser = new Parser();
    private EvaluatorVisitor _visitor = new();

    public IRuleEvaluator Compile(Token[] tokens)
    {
        var ast = _parser.Parse(tokens);
        return new RuleEvaluator(ast, _visitor);
    }

    public IRuleEvaluator Compile(string input)
    {
        var ast = _parser.Parse(input);
        return new RuleEvaluator(ast, _visitor);
    }

    public IRuleEvaluator<T> CompileAsExpression<T>(Token[] tokens) where T : JToken
    {
        var ast = _parser.ParseExpression(tokens);
        return new RuleEvaluator<T>(ast, _visitor);
    }

    public IRuleEvaluator<T> CompileAsExpression<T>(string input) where T : JToken
    {
        var ast = _parser.ParseExpression(input);
        return new RuleEvaluator<T>(ast, _visitor);
    }
}