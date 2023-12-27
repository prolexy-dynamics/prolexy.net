using FluentAssertions;
using Prolexy.Compiler.ExtensionMethods;
using Prolexy.Compiler.Models;
using Prolexy.Compiler.SchemaGenerators;

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
                }, Array.Empty<MethodData>()));
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
                    new MethodData
                    {
                        Name = "GetBirthDay",
                        Parameters = ArraySegment<ParameterData>.Empty,
                        ReturnTypeData = PrimitiveType.Datetime.GetTypeData()
                    },
                    new MethodData
                    {
                        Name = "Register",
                        Parameters = new ParameterData[]{new("reason", PrimitiveType.String.GetTypeData())},
                        ReturnTypeData = PrimitiveType.Void.GetTypeData()
                    }
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
                    new PropertyData("SimpleType", new ComplexTypeData(
                        "SimpleType",
                        new[]
                        {
                            new PropertyData("Name", PrimitiveType.String.GetTypeData()),
                            new PropertyData("Age", PrimitiveType.Number.GetTypeData()),
                            new PropertyData("RegistrationDate", PrimitiveType.Datetime.GetTypeData()),
                            new PropertyData("Accepted", PrimitiveType.Boolean.GetTypeData()),
                        }, Array.Empty<MethodData>())),
                }, Array.Empty<MethodData>()));
    }
}