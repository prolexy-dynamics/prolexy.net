namespace Prolexy.Compiler.SchemaGenerators;

public record MethodData : ITypeData
{
    public MethodData(string name, ITypeData contextType, IEnumerable<ParameterData> parameters, ITypeData returnType)
    {
        Name = name;
        ContextType = contextType;
        Parameters = parameters;
        ReturnType = returnType;
    }
    public string Name { get; set; }
    public ITypeData ContextType { get; set; }
    public TypeCategory Category => TypeCategory.Method;
    public IEnumerable<ParameterData> Parameters { get; set; }
    public ITypeData ReturnType { get; set; }
}