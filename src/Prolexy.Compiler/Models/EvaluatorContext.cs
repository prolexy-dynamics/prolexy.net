using System.Collections.Immutable;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.ExtensionMethods;

namespace Prolexy.Compiler.Models;

public record EvaluatorContext(JToken? BusinessObject, IType? Schema, ImmutableList<Module> Modules,
    ImmutableList<Method> ExtensionMethods);
