using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Prolexy.Compiler.ExtensionMethods;
using Prolexy.Compiler.Models;
using Prolexy.Compiler.SchemaGenerators;
using Tiba.Domain.Model.Uoms;
using Tiba.Trading.Domain.Contracts.Models.TradeOrders.Events;

namespace Prolexy.Compiler.Tests.SchemaGenerators;

public class SchemaGeneratorTests
{
    private SchemaGenerator sut = new SchemaGenerator();

    [Fact]
    public void Should_generate_schema_from_SimpleType()
    {
        //Arrange
        var context = ClrEvaluatorContextBuilder.Default
            .WithBusinessObject(new SimpleType())
            .Build();

        //Act
        var schema = sut.Generate(context);
        var json = JsonConvert.SerializeObject(schema, new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
        //Assert
        schema.BusinessObjectTypeData.Should()
            .BeEquivalentTo(new ComplexTypeData(
                "SimpleType",
                new[]
                {
                    new PropertyData("Name", PrimitiveType.String.GetTypeData()),
                    new PropertyData("Age", PrimitiveType.Number.GetTypeData()),
                    new PropertyData("RegistrationDate", PrimitiveType.Datetime.GetTypeData()),
                    new PropertyData("Accepted", PrimitiveType.Boolean.GetTypeData()),
                },
                Array.Empty<MethodData>(),
                new[]
                {
                    new MethodData(
                        "ctor",
                        PrimitiveType.Void.GetTypeData(null!),
                        Array.Empty<ParameterData>(),
                        new ComplexTypeReferenceDataType("SimpleType"))
                }));
    }

    [Fact]
    public void Should_generate_schema_from_SimpleTypeWithArrayProperty()
    {
        //Arrange
        var context = ClrEvaluatorContextBuilder.Default
            .WithBusinessObject(new SimpleTypeWithArray())
            .Build();

        //Act
        var schema = sut.Generate(context);

        //Assert
        schema.BusinessObjectTypeData.Should()
            .BeEquivalentTo(new ComplexTypeData(
                "SimpleTypeWithArray",
                new[]
                {
                    new PropertyData("Name", new EnumerableTypeData(PrimitiveType.String.GetTypeData())),
                    new PropertyData("Age", new EnumerableTypeData(PrimitiveType.Number.GetTypeData())),
                    new PropertyData("RegistrationDate", new EnumerableTypeData(PrimitiveType.Datetime.GetTypeData())),
                    new PropertyData("Accepted", new EnumerableTypeData(PrimitiveType.Boolean.GetTypeData())),
                },
                Array.Empty<MethodData>(),
                new[]
                {
                    new MethodData(
                        "ctor",
                        PrimitiveType.Void.GetTypeData(null),
                        new ParameterData[0],
                        new ComplexTypeReferenceDataType("SimpleTypeWithArray"))
                }));
    }

    [Fact]
    public void Should_generate_schema_from_SimpleTypeWithMethod()
    {
        //Arrange
        var context = ClrEvaluatorContextBuilder.Default
            .WithBusinessObject(new SimpleTypeWithMethod())
            .Build();

        //Act
        var schema = sut.Generate(context);

        //Assert
        schema.BusinessObjectTypeData.Should()
            .BeEquivalentTo(new ComplexTypeData(
                "SimpleTypeWithMethod",
                new[]
                {
                    new PropertyData("Name", PrimitiveType.String.GetTypeData()),
                    new PropertyData("Age", PrimitiveType.Number.GetTypeData()),
                }, new[]
                {
                    new MethodData(
                        "GetBirthDay",
                        new ComplexTypeReferenceDataType("SimpleTypeWithMethod"),
                        ArraySegment<ParameterData>.Empty,
                        PrimitiveType.Datetime.GetTypeData()
                    ),
                    new MethodData(
                        "Register",
                        new ComplexTypeReferenceDataType("SimpleTypeWithMethod"),
                        new ParameterData[] { new("reason", PrimitiveType.String.GetTypeData()) },
                        PrimitiveType.Void.GetTypeData()
                    )
                },
                new[]
                {
                    new MethodData(
                        "ctor",
                        PrimitiveType.Void.GetTypeData(null!),
                        Array.Empty<ParameterData>(),
                        new ComplexTypeReferenceDataType("SimpleTypeWithMethod"))
                }));
    }

    [Fact]
    public void Should_generate_schema_from_TypeComposedFromSimpleType()
    {
        //Arrange
        var context = ClrEvaluatorContextBuilder.Default
            .WithBusinessObject(new TypeComposedFromSimpleType())
            .Build();

        //Act
        var schema = sut.Generate(context);

        //Assert
        schema.BusinessObjectTypeData.Should()
            .BeEquivalentTo(new ComplexTypeData(
                "TypeComposedFromSimpleType",
                new[]
                {
                    new PropertyData("SimpleType", new ComplexTypeReferenceDataType("SimpleType")),
                },
                Array.Empty<MethodData>(),
                new []{
                    new MethodData(
                        "ctor",
                        PrimitiveType.Void.GetTypeData(null!),
                        Array.Empty<ParameterData>(),
                        new ComplexTypeReferenceDataType("TypeComposedFromSimpleType"))
                }));

        schema.ComplexDataTypes
            .Single(t => t.Name == "SimpleType").Should()
            .BeEquivalentTo(new ComplexTypeData(
                "SimpleType",
                new[]
                {
                    new PropertyData("Name", PrimitiveType.String.GetTypeData()),
                    new PropertyData("Age", PrimitiveType.Number.GetTypeData()),
                    new PropertyData("RegistrationDate", PrimitiveType.Datetime.GetTypeData()),
                    new PropertyData("Accepted", PrimitiveType.Boolean.GetTypeData()),
                },
                Array.Empty<MethodData>(),
                new[]
                {
                    new MethodData(
                        "ctor",
                        PrimitiveType.Void.GetTypeData(null!),
                        Array.Empty<ParameterData>(),
                        new ComplexTypeReferenceDataType("SimpleType"))
                }));
    }

    [Fact]
    public void Should_generate_schema_from_TibaTradingEvent()
    {
        //Arrange
        var context = ClrEvaluatorContextBuilder.Default
            .WithBusinessObject(new SimpleTypeWithMethod())
            .Build();

        //Act
        var schema = sut.Generate(context);
        var json = JsonConvert.SerializeObject(schema, new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
        //Assert
    }
}