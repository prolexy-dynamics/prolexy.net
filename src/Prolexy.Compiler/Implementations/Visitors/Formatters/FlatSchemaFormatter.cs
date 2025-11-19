using Newtonsoft.Json.Linq;

namespace Prolexy.Compiler.Implementations.Visitors.Formatters;

public class FlatSchemaFormatter
{
    public JObject Format(LogicalSchemaNode ir)
    {
        var result = new JObject();

        foreach (var kv in ir.Properties)
            result[kv.Key] = FormatNode(kv.Value);

        return result;
    }

    private JToken FormatNode(LogicalSchemaNode n)
    {
        return n.Kind switch
        {
            LogicalSchemaNode.NodeKind.Primitive => new JValue(n.PrimitiveType),
            LogicalSchemaNode.NodeKind.Object => FormatObject(n),
            LogicalSchemaNode.NodeKind.Array => new JArray(FormatObject(n.Item)),
            _ => throw new Exception("Invalid IR schema node")
        };
    }

    private JObject FormatObject(LogicalSchemaNode n)
    {
        var obj = new JObject();

        foreach (var kv in n.Properties)
            obj[kv.Key] = FormatNode(kv.Value);

        return obj;
    }
}