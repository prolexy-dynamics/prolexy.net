using FluentAssertions;
using Newtonsoft.Json.Linq;
using Prolexy.Compiler.Implementations;
using Prolexy.Compiler.Implementations.Visitors;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.Tests.LogicalSchemaGenerators;

public class IRSchemaMergeTests
{
    [Fact]
    public void MergeWith_should_merge_two_object_nodes()
    {
        var a = LogicalSchemaNode.Object();
        a.Properties["name"] = LogicalSchemaNode.Primitive("string");

        var b = LogicalSchemaNode.Object();
        b.Properties["age"] = LogicalSchemaNode.Primitive("number");

        var result = a.MergeWith(b);

        result.Kind.Should().Be(LogicalSchemaNode.NodeKind.Object);
        result.Properties.Should().ContainKey("name");
        result.Properties.Should().ContainKey("age");
    }
    [Fact]
    public void MergeWith_should_override_primitive_fields()
    {
        var a = LogicalSchemaNode.Primitive("string");
        var b = LogicalSchemaNode.Primitive("number");

        var result = a.MergeWith(b);

        result.PrimitiveType.Should().Be("number");
    }
    [Fact]
    public void MergeWith_should_merge_array_item_properties()
    {
        var arr1 = LogicalSchemaNode.Array(
            LogicalSchemaNode.Object()
        );
        arr1.Item.Properties["Product"] = LogicalSchemaNode.Primitive("string");

        var arr2 = LogicalSchemaNode.Array(
            LogicalSchemaNode.Object()
        );
        arr2.Item.Properties["Quantity"] = LogicalSchemaNode.Primitive("number");

        var result = arr1.MergeWith(arr2);

        var item = result.Item;

        item.Properties["Product"].PrimitiveType.Should().Be("string");
        item.Properties["Quantity"].PrimitiveType.Should().Be("number");
    }
    [Fact]
    public void MergeWith_should_merge_LineItems_items()
    {
        var schema1 = ParseAndVisit("LineItems.Exists(def x => x.Product is 'Book')");
        var schema2 = ParseAndVisit("LineItems.Exists(def x => x.Quantity > 0)");

        var merged = schema1.MergeWith(schema2);

        var item = merged.Properties["LineItems"].Item;

        item.Properties.Should().ContainKey("Quantity");
        item.Properties.Should().ContainKey("Product");

        item.Properties["Product"].PrimitiveType.Should().Be("string");
        item.Properties["Quantity"].PrimitiveType.Should().Be("number");
    }

    [Fact]
    public void MergeWith_should_not_modify_original_nodes()
    {
        var schema1 = ParseAndVisit("LineItems.Exists(def x => x.Product is 'Book')");
        var schema2 = ParseAndVisit("LineItems.Exists(def x => x.Quantity > 0)");

        var merged = schema1.MergeWith(schema2);

        // original must remain unchanged
        schema1.Properties["LineItems"].Item.Properties.Should().ContainKey("Product");
        schema1.Properties["LineItems"].Item.Properties.Should().NotContainKey("Quantity");

        schema2.Properties["LineItems"].Item.Properties.Should().ContainKey("Quantity");
        schema2.Properties["LineItems"].Item.Properties.Should().NotContainKey("Product");
    }
    private LogicalSchemaNode ParseAndVisit(string expr, bool statement = false)
    {
        var parser = new Parser(); // پارسر اصلی شما
        var ast = statement ? parser.Parse(expr) : parser.ParseExpression(expr);
        return ast.Visit(_visitor, _context);
    }
    private readonly LogicalSchemaVisitor _visitor = new();

    public class MyBusinessObject
    {
        public LogicalSchemaVisitorTests.LineItem[] LineItems { get; set; }
        public bool accepted { get; set; }
        public JObject AdditionalData { get; set; }
        public JObject brother { get; set; }
    }

    public record LineItem(string Product, int Quantity);

    private readonly ClrEvaluatorContext _context = EvaluatorContextBuilder
        .Default
        .AsClrEvaluatorBuilder()
        .WithBusinessObject(new LogicalSchemaVisitorTests.MyBusinessObject())
        .Build();

}