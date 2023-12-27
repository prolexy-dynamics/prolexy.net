using System.Collections.Immutable;
using System.Reflection;
using Prolexy.Compiler.ExtensionMethods;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.SchemaGenerators;

public class SchemaGenerator
{
    public ContextSchema Generate(IEvaluatorContext context)
    {
        var type = context.BusinessObject.GetType();
        var complexSchema = FromClassType(type);
        return new ContextSchema
        {
            BusinessObjectTypeData = complexSchema,
            ExtensionMethods = GenerateExtensionMethods(context.ExtensionMethods)
        };
    }

    private IEnumerable<MethodData> GenerateExtensionMethods(ImmutableList<Method> methods)
    {
        return methods.Select(m => new MethodData
        {
            Name = m.Name,
            Parameters = m.Parameters.Select(p => new ParameterData(p.ParameterName,  p.ParameterType.GetTypeData(this))),
            ReturnTypeData = m.ReturnType.GetTypeData(this)
        });
    }

    private PropertyData[] GenerateProperties(Type type)
    {
        return type.GetProperties()
            .Select(p => new PropertyData(p.Name, FromClrType(p.PropertyType)))
            .ToArray();
    }


    public ITypeData FromClrType(Type type)
    {
        if (type.IsPrimitive)
            return FromPrimitiveType(type);
        if (type == typeof(string))
            return PrimitiveType.String.GetTypeData(this);
        if (type == typeof(DateTime))
            return PrimitiveType.Datetime.GetTypeData(this);
        if (type == typeof(void))
            return PrimitiveType.Void.GetTypeData(this);
        if (type.IsClass)
            return FromClassType(type);
        return null!;
    }

    ComplexTypeData FromClassType(Type type)
    {
        var properties = GenerateProperties(type);
        var methods = GenerateMethods(type);
        return new ComplexTypeData(type.Name, properties, methods);
    }

    private IEnumerable<MethodData> GenerateMethods(Type type)
    {
        return type.GetMethods()
            .Where(m => m.DeclaringType == type && !m.IsSpecialName)
            .Select(m => new MethodData
            {
                Name = m.Name,
                ReturnTypeData = FromClrType(m.ReturnType),
                Parameters = m.GetParameters().Select(p => new ParameterData(p.Name, FromClrType(p.ParameterType)))
            })
            .Where(m => m.ReturnTypeData != null);
    }

    private ITypeData FromPrimitiveType(Type type)
    {
        if (type == typeof(decimal) ||
            type == typeof(long) ||
            type == typeof(int) ||
            type == typeof(short) ||
            type == typeof(byte))
            return PrimitiveType.Number.GetTypeData(this);

        if (type.IsAssignableTo(typeof(bool)))
            return PrimitiveType.Boolean.GetTypeData(this);
        throw new Exception("prolexy type not found exception.");
    }
}