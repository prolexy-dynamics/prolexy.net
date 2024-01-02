using System.Collections;
using System.Collections.Immutable;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.ExtensionMethods;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.SchemaGenerators;

public class SchemaGenerator
{
    private Dictionary<Type, ComplexTypeReferenceDataType> _visited = new();
    public ImmutableArray<ComplexTypeData> ComplexTypes { get; private set; } = ImmutableArray<ComplexTypeData>.Empty;

    public ContextSchema Generate(IEvaluatorContext context)
    {
        var type = context.BusinessObject.GetType();
        var referenceDataType = FromClassType(type);
        return new ContextSchema
        {
            BusinessObjectTypeData = ComplexTypes.Single(t => t.Name == referenceDataType.Name),
            ComplexDataTypes = ComplexTypes,
            ExtensionMethods = GenerateExtensionMethods(context.ExtensionMethods)
        };
    }

    private IEnumerable<MethodData> GenerateExtensionMethods(ImmutableList<Method> methods)
    {
        return methods.Select(m => new MethodData(m.Name,
            m.ContextType.GetTypeData(this),
            m.Parameters
                .Select(p => new ParameterData(p.ParameterName, p.ParameterType.GetTypeData(this)))
                .ToList(),
            m.ReturnType.GetTypeData(this)));
    }

    private PropertyData[] GenerateProperties(Type type)
    {
        return type.GetProperties()
            .Where(p => !IsIgnoredType(p.PropertyType))
            .Select(p => new PropertyData(p.Name, FromClrType(p.PropertyType)))
            .ToArray();
    }


    public ITypeData FromClrType(Type type)
    {
        if (type.IsPrimitive)
            return FromPrimitiveType(type);
        if (type == typeof(string))
            return PrimitiveType.String.GetTypeData(this);
        if (type == typeof(Decimal))
            return PrimitiveType.Number.GetTypeData(this);
        if (type == typeof(Guid))
            return PrimitiveType.String.GetTypeData(this);
        if (type == typeof(DateTime))
            return PrimitiveType.Datetime.GetTypeData(this);
        if (type == typeof(void))
            return PrimitiveType.Void.GetTypeData(this);
        if (type.IsAssignableTo(typeof(IEnumerable)))
            return new EnumerableTypeData(FromClrType(GetElementType(type)));
        if (type.IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(Nullable<>))
            return new EnumerableTypeData(FromClrType(type.GetGenericArguments()[0]));
        if (type.IsClass || type.IsValueType)
            return FromClassType(type);
        if (type.IsEnum)
            return new EnumTypeData(type.Name, Enum.GetValues(type).Cast<string>());
        return null!;
    }

    Type GetElementType(Type type)
    {
        return type.GetInterfaces().First(t =>
                t.GetGenericArguments().Length == 1 &&
                t.IsAssignableTo(typeof(IEnumerable<>).MakeGenericType(t.GetGenericArguments()[0])))
            .GetGenericArguments()[0];
    }

    ComplexTypeReferenceDataType FromClassType(Type type)
    {
        if (_visited.TryGetValue(type, out var cType))
            return cType;
        var result = new ComplexTypeReferenceDataType("");
        _visited[type] = result;
        var properties = GenerateProperties(type);
        var methods = GenerateMethods(type);
        var constructors = GenerateConstructors(type);
        var complexType = new ComplexTypeData(type.Name, properties, methods, constructors);
        ComplexTypes = ComplexTypes.Add(complexType);
        result.Name = complexType.Name;
        return result;
    }

    private IEnumerable<MethodData> GenerateConstructors(Type type)
    {
        return type.GetConstructors()
            .Select(c => new MethodData(
                "ctor",
                PrimitiveType.Void.GetTypeData(),
                c.GetParameters().Select((p, idx) => new ParameterData(p.Name ?? $"p{idx}", FromClrType(p.ParameterType))),
                FromClrType(type)));
    }

    bool IsIgnoredType(Type type) => type.IsAssignableTo(typeof(JToken)) ||
                                     type == typeof(object) ||
                                     type.IsInterface ||
                                     type == typeof(CultureInfo) ||
                                     (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>));

    private IEnumerable<MethodData> GenerateMethods(Type type)
    {
        return type.GetMethods()
            .Where(m => m.DeclaringType == type && !m.IsSpecialName &&
                        !IsIgnoredType(m.ReturnType) &&
                        m.GetParameters().All(p => !IsIgnoredType(p.ParameterType)))
            .Select(m => new MethodData(m.Name,
                new ComplexTypeReferenceDataType(type.Name),
                m.GetParameters()
                    .Select(p => new ParameterData(p.Name, FromClrType(p.ParameterType)))
                    .ToList(),
                FromClrType(m.ReturnType)))
            .Where(m => m.ReturnType != null)
            .ToList();
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

public class ComplexTypeReferenceDataType : ITypeData
{
    public ComplexTypeReferenceDataType(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
    public TypeCategory Category => TypeCategory.ReferenceType;
}