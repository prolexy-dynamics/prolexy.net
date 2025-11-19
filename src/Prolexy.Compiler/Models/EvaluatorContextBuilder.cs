using System.Collections;
using System.Collections.Immutable;
using System.Reflection;
using Prolexy.Compiler.ExtensionMethods;
using Prolexy.Compiler.ExtensionMethods.DateTimeExtensions;

namespace Prolexy.Compiler.Models;

public record EvaluatorContextBuilder
{
    public static EvaluatorContextBuilder Default => new EvaluatorContextBuilder()
        .ScanAssemblyForExtensionMethod(typeof(AddDaysMethod).Assembly);

    ImmutableList<Method> _extensionMethods = ImmutableList<Method>.Empty;
    private readonly ImmutableList<Module> _modules = ImmutableList<Module>.Empty;

    public EvaluatorContextBuilder WithExtensionMethod(Method method)
    {
        _extensionMethods = _extensionMethods.Add(method);
        return this;
    }

    private Hashtable visitedAssembly = new();

    public EvaluatorContextBuilder ScanAssemblyForExtensionMethod(Assembly assembly)
    {
        if (visitedAssembly.ContainsKey(assembly)) return this;
        visitedAssembly[assembly] = true;
        foreach (var type in assembly.GetExportedTypes()
                     .Where(t => typeof(Method).IsAssignableFrom(t) && !t.IsAbstract))
        {
            if (type.GetConstructors().Any(c => c.GetParameters().Length == 0) &&
                Activator.CreateInstance(type) is Method method)
                WithExtensionMethod(method);
        }

        return this;
    }

    public JsonEvaluatorContextBuilder AsJsonEvaluatorBuilder() => new(_modules, _extensionMethods);
    public ClrEvaluatorContextBuilder AsClrEvaluatorBuilder() => new(_modules, _extensionMethods);
}