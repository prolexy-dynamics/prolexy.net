using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Models;
using Prolexy.Compiler.SchemaGenerators;

namespace Prolexy.Compiler.ExtensionMethods;

public record MethodSignature(IType ContextType, IEnumerable<Parameter> Parameters, IType ReturnType) : IMethod
{
    public string Name => "AnonymousMethod";
    public IType? GetPropertyType(string name)
    {
        return null;
    }

    public ITypeData GetTypeData(SchemaGenerator generator)
    {
        return new MethodData( Name,
            ContextType.GetTypeData(generator),
            Parameters.Select(p => new ParameterData(p.ParameterName, p.ParameterType.GetTypeData(generator))),
            ReturnType.GetTypeData(generator)
        );
    }

    public object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context, object methodContext, IEnumerable<IAst> args)
    {
        return null;
    }
}