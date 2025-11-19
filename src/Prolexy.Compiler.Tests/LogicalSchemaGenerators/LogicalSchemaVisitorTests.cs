using FluentAssertions;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Implementations.Visitors;
using Prolexy.Compiler.Implementations.Visitors.Formatters;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.Tests.LogicalSchemaGenerators;

public class LogicalSchemaVisitorTests
{
    private readonly LogicalSchemaVisitor _visitor = new();

    public class MyBusinessObject
    {
        public LineItem[] LineItems { get; set; }
        public bool accepted { get; set; }
        public JObject AdditionalData { get; set; }
        public JObject brother { get; set; }
    }

    public record LineItem(string Product, int Quantity);

    private readonly ClrEvaluatorContext _context = EvaluatorContextBuilder
        .Default
        .AsClrEvaluatorBuilder()
        .WithBusinessObject(new MyBusinessObject())
        .Build();


    private LogicalSchemaNode ParseAndVisit(string expr, bool statement = false)
    {
        var parser = new Parser(); // پارسر اصلی شما
        var ast = statement ? parser.Parse(expr) : parser.ParseExpression(expr);
        return ast.Visit(_visitor, _context);
    }

    // ------------------------------------------------------------------
    // 1. تست ساده: فیلدهای primitive
    // ------------------------------------------------------------------

    [Fact]
    public void IR_should_collect_simple_fields()
    {
        var ir = ParseAndVisit("accepted is true");

        ir.Kind.Should().Be(LogicalSchemaNode.NodeKind.Object);
        ir.Properties.Should().ContainKey("accepted");
        ir.Properties["accepted"].PrimitiveType.Should().Be("boolean");
    }

    // ------------------------------------------------------------------
    // 2. nested fields → زیر JObject
    // ------------------------------------------------------------------

    [Fact]
    public void IR_should_collect_nested_fields()
    {
        var ir = ParseAndVisit("brother.birthDay before 2020/10/12");

        ir.Properties.Should().ContainKey("brother");
        var bro = ir.Properties["brother"];

        bro.Kind.Should().Be(LogicalSchemaNode.NodeKind.Object);
        bro.Properties.Should().ContainKey("birthDay");

        bro.Properties["birthDay"].PrimitiveType.Should().Be("date-time");
    }

    // ------------------------------------------------------------------
    // 3. array with lambda parameter (x disappears)
    // ------------------------------------------------------------------

    [Fact]
    public void IR_should_collect_array_fields()
    {
        var ir = ParseAndVisit("LineItems.Exists(def x => x.Product is 'special')");

        ir.Properties.Should().ContainKey("LineItems");

        var arr = ir.Properties["LineItems"];
        arr.Kind.Should().Be(LogicalSchemaNode.NodeKind.Array);

        var item = arr.Item;
        item.Kind.Should().Be(LogicalSchemaNode.NodeKind.Object);
        item.Properties.Should().ContainKey("Product");
        item.Properties["Product"].PrimitiveType.Should().Be("string");
    }

    // ------------------------------------------------------------------
    // 4. array combined with object field
    // ------------------------------------------------------------------

    [Fact]
    public void IR_should_support_combined_fields()
    {
        var ir = ParseAndVisit(
            "brother.birthDay before 2020/10/12 and LineItems.Exists(def x => x.Quantity > 0)");

        // brother
        ir.Properties.Should().ContainKey("brother");
        ir.Properties["brother"].Properties.Should().ContainKey("birthDay");
        ir.Properties["brother"].Properties["birthDay"].PrimitiveType.Should().Be("date-time");

        // array
        ir.Properties.Should().ContainKey("LineItems");
        ir.Properties["LineItems"].Kind.Should().Be(LogicalSchemaNode.NodeKind.Array);

        var item = ir.Properties["LineItems"].Item;
        item.Properties.Should().ContainKey("Quantity");
        item.Properties["Quantity"].PrimitiveType.Should().Be("number");
    }

    // ------------------------------------------------------------------
    // 5. Test JSON Schema Formatter
    // ------------------------------------------------------------------

    [Fact]
    public void JsonSchemaFormatter_should_build_valid_schema()
    {
        var ir = ParseAndVisit("LineItems.Exists(def x => x.Quantity > 0)");

        var json = new JsonSchemaFormatter().Format(ir);
        var props = (JObject)json["properties"];
        var lineItems = (JObject)props["LineItems"];

        lineItems["type"].Value<string>().Should().Be("array");
        var qty = (JObject)lineItems["items"]["properties"]["Quantity"];
        qty["type"].Value<string>().Should().Be("number");
    }

    // ------------------------------------------------------------------
    // 6. Test Flat Schema Formatter
    // ------------------------------------------------------------------

    [Fact]
    public void FlatSchemaFormatter_should_build_simple_schema()
    {
        var ir = ParseAndVisit("LineItems.Exists(def x => x.Product is 'x')");

        var flat = new FlatSchemaFormatter().Format(ir);

        var arr = (JArray)flat["LineItems"];
        var first = (JObject)arr[0];

        first["Product"].Value<string>().Should().Be("string");
    }
    
    [Fact]
    public void IR_should_build_nested_properties_under_JObject()
    {
        var ir = ParseAndVisit("AdditionalData.father.name is 'Joe'");

        var add = ir.Properties["AdditionalData"];
        var father = add.Properties["father"];
        var name = father.Properties["name"];

        name.PrimitiveType.Should().Be("string");
    }

    [Fact]
    public void IR_should_guess_type_from_name_when_no_model_info()
    {
        var ir = ParseAndVisit("brother.birthDay before 2020/10/12");

        ir.Properties["brother"].Properties["birthDay"]
            .PrimitiveType.Should().Be("date-time");
    }

    [Fact]
    public void IR_should_merge_multiple_properties_inside_lambda_item()
    {
        var ir = ParseAndVisit(
            "LineItems.Exists(def x => x.Quantity > 0 and x.Product is 'x')");

        var item = ir.Properties["LineItems"].Item;
    
        item.Properties["Quantity"].PrimitiveType.Should().Be("number");
        item.Properties["Product"].PrimitiveType.Should().Be("string");
    }

    [Fact]
    public void IR_lambda_parameter_should_not_appear_in_output()
    {
        var ir = ParseAndVisit("LineItems.Exists(def x => x.Quantity > 0)");

        var item = ir.Properties["LineItems"].Item;

        item.Properties.Keys.Should().NotContain("x");
    }

    [Fact]
    public void IR_should_read_property_type_from_model()
    {
        var ir = ParseAndVisit("LineItems.Exists(def x => x.Quantity > 0)");

        ir.Properties["LineItems"].Item.Properties["Quantity"]
            .PrimitiveType.Should().Be("number");
    }

    [Fact]
    public void IR_should_merge_multiple_properties_in_same_object()
    {
        var ir = ParseAndVisit(
            "brother.birthDay before 2020/10/12 and brother.age > 10");

        var bro = ir.Properties["brother"];

        bro.Properties["birthDay"].PrimitiveType.Should().Be("date-time");
        bro.Properties["age"].PrimitiveType.Should().Be("string");
    }

    [Fact]
    public void JsonSchemaFormatter_should_render_nested_and_array_combined()
    {
        var ir = ParseAndVisit(
            "brother.birthDay before 2020/10/12 and LineItems.Exists(def x => x.Quantity > 0)");

        var json = new JsonSchemaFormatter().Format(ir);
        var props = (JObject)json["properties"];

        ((JObject)props["brother"]["properties"]["birthDay"])["format"]
            .Value<string>().Should().Be("date-time");

        ((JObject)props["LineItems"]["items"]["properties"]["Quantity"])["type"]
            .Value<string>().Should().Be("number");
    }

}
