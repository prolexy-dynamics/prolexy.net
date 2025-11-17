using FluentAssertions;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Models;
using Prolexy.Compiler.Tests.Evaluator;
using Prolexy.Compiler.Tests.SchemaGenerators;

namespace Prolexy.Compiler.Tests.ExpressionFieldVisitors;

public class ExpressionFieldVisitorTests
{
    [Fact]
    public void Should_extract_single_field_from_expression()
    {
        // Name is compared to constant
        var fields = ExtractFields("Name is 'Yasser'", false);
        fields.Should().BeEquivalentTo(new Dictionary<string, object>
        {
            { "Name", "string" }
        });
    }

    [Fact]
    public void Should_extract_multiple_fields_from_expression()
    {
        var fields = ExtractFields("Name + ' ' + Family == 'Test'", false);
        fields.Should().BeEquivalentTo(new Dictionary<string, object>
        {
            { "Name", "string" },
            { "Family", "string" },
        });
    }

    [Fact]
    public void Should_extract_nested_fields()
    {
        var fields = ExtractFields("brother.birthDay before 2020/10/12", false);
        fields.Should().BeEquivalentTo(new Dictionary<string, object>
        {
            {
                "brother", new Dictionary<string, object>
                {
                    { "birthDay", "datetime" },
                }
            }
        });
    }

    [Fact]
    public void Should_extract_fields_in_if_statement()
    {
        var fields = ExtractFields("if(age > 18) then set adult with true end");
        fields.Should().BeEquivalentTo(new Dictionary<string, object>
        {
            { "age", "number" }
        });
    }

    [Fact]
    public void Should_extract_fields_from_complex_condition()
    {
        var fields =
            ExtractFields(
                "set Discount with CouponKey contains '2001' and LineItems.Exists(def x => x.Product is 'special' and x.Address is 'Tehran')");

        fields.Should().BeEquivalentTo(new Dictionary<string, object>
        {
            { "CouponKey", "string" },
            {
                "LineItems", new List<Dictionary<string, object>>
                {
                    new()
                    {
                        { "Product", "string" },
                        { "Address", "string" },
                    }
                }
            }
        });
    }

    [Fact]
    public void Should_extract_fields_from_assignment_statement()
    {
        var fields = ExtractFields("set fullname with name + ' ' + family");
        fields.Should().BeEquivalentTo(new Dictionary<string, object>
        {
            { "name", "string" },
            { "family", "string" },
        });
    }

    [Fact]
    public void Should_extract_fields_from_chained_statements()
    {
        var fields = ExtractFields("""
                                   if(age > 10) then 
                                        set brother.age with age + 10 and then
                                        set age with brother.age + 10 and then
                                        set fullname with name + ' ' + family
                                   end
                                   """);
        fields.Should().BeEquivalentTo(new Dictionary<string, object>
        {
            { "age", "number" },
            { "name", "string" },
            { "family", "string" },
            {
                "brother", new Dictionary<string, object>
                {
                    { "age", "number" },
                }
            }
        });
    }

    [Fact]
    public void Should_extract_fields_from_object_access_in_expression()
    {
        var fields =
            ExtractFields(
                "if(AdditionalData.father.incomes.Sum(def x => x.Amount + 10)) then set brother.age with age + 10 end");
        JObject.FromObject(fields).Should().BeEquivalentTo(
            JObject.Parse("""
                          {
                             "age": "number",
                             "AdditionalData": {"father": {"incomes": [{"Amount": "number"}]}}
                          }
                          """
            ));
    }

// Helper (stubbed — the actual implementation will come from your visitor)
    private Dictionary<string, object> ExtractFields(string expression, bool statement = true)
    {
        var compiler = new Implementations.Compiler();
        var evaluator = statement
            ? compiler.Compile(expression).AsFieldCollectorEvaluator()
            : compiler.CompileExpression(expression).AsFieldCollectorEvaluator();
        var ctx = EvaluatorContextBuilder.Default
            .AsClrEvaluatorBuilder()
            .WithBusinessObject(new SimpleType())
            .AddClrType<SimpleType>()
            .Build();
        var result = evaluator.Evaluate(ctx)!;
        return result.Fields;
    }
}