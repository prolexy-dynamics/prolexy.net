using System.Collections.Immutable;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.ExtensionMethods;
using Prolexy.Compiler.Models;
using Prolexy.Compiler.SchemaGenerators;

namespace Prolexy.Compiler.Visitors.TypeDetectorVisitors;

public interface IExpressionTypeDetectorVisitor : IAstVisitor<ExpressionTypeDetectorContext, TypeDetectorResult>
{
    TypeDetectorResult Visit(IAst ast, ExpressionTypeDetectorContext context);
}

public record ExpressionTypeDetectorContext(
        object BusinessObject,
        ImmutableList<Module> Modules, 
        ImmutableList<Method> ExtensionMethods,
        ImmutableList<Type> ClrTypes)
    : IEvaluatorContext
{
    public Stack<Dictionary<string, object>> Variables { get; } = new();
}

public record TypeDetectorResult(ExpressionTypeDetectorContext Context, Type? Result) : IEvaluatorResult
{
    IEvaluatorContext  IEvaluatorResult.Context
    {
        get => Context;
        init => Context = (ExpressionTypeDetectorContext)value;
    }
    object IEvaluatorResult.Value { get; init; }
}