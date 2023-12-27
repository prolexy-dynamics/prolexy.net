using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Models;
using Prolexy.Compiler.SchemaGenerators;

namespace Prolexy.Compiler.ExtensionMethods;

public record MethodSignature(IEnumerable<Parameter> Parameters, IType ReturnType) : IMethod
{
    public string Name => "AnonymousMethod";
    public IType? GetPropertyType(string name)
    {
        return null;
    }

    public ITypeData GetTypeData(SchemaGenerator generator)
    {
        return null;
    }

    public object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context, object methodContext, IEnumerable<IAst> args)
    {
        return null;
    }
}
public abstract record Method : IMethod
{
    protected Method(string Name, IEnumerable<Parameter> Parameters, IType ReturnType)
    {
        this.Name = Name;
        this.MutableParameters = Parameters.ToList();
        this.ReturnType = ReturnType;
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
        return new MethodData
        {
            Name = Name,
            Parameters = Parameters.Select(p => new ParameterData(p.ParameterName, p.ParameterType.GetTypeData(generator))),
            ReturnTypeData = ReturnType.GetTypeData(generator)
        };
    }

    public abstract bool Accept(object value);

    public virtual bool Equal(Method other)
    {
        return this.GetType() == other.GetType() && Name == other.Name;
    }

    public string Name { get; init; }
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