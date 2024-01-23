using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Implementations;
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
public abstract record Method : IMethod
{
    protected Method(string name, IType contextType, IEnumerable<Parameter> parameters, IType returnType)
    {
        Name = name;
        ContextType = contextType;
        MutableParameters = parameters.ToList();
        ReturnType = returnType;
    }

    public abstract object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context,
        object methodContext,
        IEnumerable<IAst> args);

    public IType? GetPropertyType(string name)
    {
        return ReturnType;
    }

    public ITypeData GetTypeData(SchemaGenerator generator)
    {
        return new MethodData(
             Name,
             ContextType.GetTypeData(generator),
             Parameters.Select(p => new ParameterData(p.ParameterName, p.ParameterType.GetTypeData(generator))),
             ReturnType.GetTypeData(generator)
        );
    }

    public abstract bool Accept(object value, bool implicitAccessMethod);

    public virtual bool Equal(Method other)
    {
        return this.GetType() == other.GetType() && Name == other.Name;
    }


    public string Name { get; init; }
    public IType ContextType { get; init; }
    protected List<Parameter> MutableParameters { get; init; }
    public IEnumerable<Parameter> Parameters => MutableParameters;
    public IType ReturnType { get; init; }

    public void Deconstruct(out string Name, out IEnumerable<Parameter> Parameters, out IType ReturnType)
    {
        Name = this.Name;
        Parameters = this.Parameters;
        ReturnType = this.ReturnType;
    }
    
    
}

public record Parameter(string ParameterName, IType ParameterType)
{
}

public interface IMethod : IType
{
    object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context,
        object methodContext, IEnumerable<IAst> args);
}