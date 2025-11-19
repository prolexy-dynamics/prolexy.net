using System.Collections.Immutable;
using Prolexy.Compiler.ExtensionMethods;
using Prolexy.Compiler.Models;
using Prolexy.Compiler.SchemaGenerators;

namespace Prolexy.Compiler.Visitors.TypeDetectorVisitors;

public record ExpressionTypeDetectorContext(
        Type BusinessObjectType,
        ImmutableList<Module> Modules, 
        ImmutableList<Method> ExtensionMethods,
        ImmutableList<ClrType> ClrTypes)
    : IEvaluatorContext
{
    public Stack<Dictionary<string, Type>> Variables { get; } = new();
    Stack<Dictionary<string, object>> IEvaluatorContext.Variables { get; } = new();
    object IEvaluatorContext.BusinessObject { get; init; }
}