namespace Prolexy.Compiler.SchemaGenerators;

public record ParameterData
{
    public ParameterData(string parameterName, ITypeData parameterType)
    {
        ParameterName = parameterName;
        ParameterType = parameterType;
    }
    public string ParameterName { get; set; }
    public ITypeData ParameterType { get; set; }
}