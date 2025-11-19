using System.Collections.Immutable;
using Prolexy.Compiler.ExtensionMethods;

namespace Prolexy.Compiler.Models;

public interface IEvaluatorContext
{
    Stack<Dictionary<string, object>>  Variables { get; }
    object BusinessObject { get; init; }
    ImmutableList<Module> Modules { get; init; }
    ImmutableList<Method> ExtensionMethods { get; init; }
}