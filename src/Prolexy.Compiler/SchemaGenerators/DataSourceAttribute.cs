namespace Prolexy.Compiler.SchemaGenerators;

public class DataSourceAttribute : Attribute
{
    public Type TargetType { get; }

    public DataSourceAttribute(Type targetType)
    {
        TargetType = targetType;
    }
}