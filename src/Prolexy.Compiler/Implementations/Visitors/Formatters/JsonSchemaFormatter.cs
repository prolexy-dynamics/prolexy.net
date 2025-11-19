using Newtonsoft.Json.Linq;

namespace Prolexy.Compiler.Implementations.Visitors.Formatters;

public class JsonSchemaFormatter
{
    public JObject Format(LogicalSchemaNode ir)
    {
        return new JObject
        {
            ["$schema"] = "http://json-schema.org/draft-07/schema#",
            ["type"] = "object",
            ["properties"] = FormatObject(ir)
        };
    }

    private JObject FormatObject(LogicalSchemaNode n)
    {
        var result = new JObject();

        foreach (var kv in n.Properties)
            result[kv.Key] = FormatNode(kv.Value);

        return result;
    }

    private JObject FormatNode(LogicalSchemaNode n)
    {
        return n.Kind switch
        {
            LogicalSchemaNode.NodeKind.Object => FormatObjectSchema(n),
            LogicalSchemaNode.NodeKind.Array  => FormatArraySchema(n),
            LogicalSchemaNode.NodeKind.Primitive => FormatPrimitiveSchema(n),
            _ => throw new Exception("Invalid IR schema node")
        };
    }

    private JObject FormatObjectSchema(LogicalSchemaNode n)
    {
        return new JObject
        {
            ["type"] = "object",
            ["properties"] = FormatObject(n)
        };
    }

    private JObject FormatArraySchema(LogicalSchemaNode n)
    {
        return new JObject
        {
            ["type"] = "array",
            ["items"] = FormatNode(n.Item)
        };
    }

    private JObject FormatPrimitiveSchema(LogicalSchemaNode n)
    {
        if (n.PrimitiveType == "date-time")
            return new JObject { ["type"] = "string", ["format"] = "date-time" };

        return new JObject { ["type"] = n.PrimitiveType };
    }
}