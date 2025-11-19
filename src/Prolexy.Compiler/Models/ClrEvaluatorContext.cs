using System.Collections.Immutable;
using Prolexy.Compiler.ExtensionMethods;

namespace Prolexy.Compiler.Models;

public record ClrEvaluatorContext(object BusinessObject, 
    ImmutableList<ClrType> ClrTypes,
    ImmutableList<Module> Modules,
    ImmutableList<Method> ExtensionMethods) : IEvaluatorContext
{
    public Stack<Dictionary<string, object>> Variables { get; } = new();
}