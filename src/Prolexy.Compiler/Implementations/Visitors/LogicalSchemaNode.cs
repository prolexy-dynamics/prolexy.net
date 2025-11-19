namespace Prolexy.Compiler.Implementations.Visitors;

public class LogicalSchemaNode
{
    public enum NodeKind
    {
        Object,
        Array,
        Primitive
    }

    public NodeKind Kind { get; set; }

    // اگر Object
    public Dictionary<string, LogicalSchemaNode> Properties { get; set; }

    // اگر Array
    public LogicalSchemaNode Item { get; set; }

    // اگر Primitive
    public string PrimitiveType { get; set; }

    public static LogicalSchemaNode Primitive(string t)
        => new LogicalSchemaNode { Kind = NodeKind.Primitive, PrimitiveType = t };

    public static LogicalSchemaNode Object()
        => new LogicalSchemaNode { Kind = NodeKind.Object, Properties = new Dictionary<string, LogicalSchemaNode>() };

    public static LogicalSchemaNode Array(LogicalSchemaNode item)
        => new LogicalSchemaNode { Kind = NodeKind.Array, Item = item };

    public LogicalSchemaNode MergeWith(LogicalSchemaNode other)
    {
        return MergeNodes(Clone(this), Clone(other));
    }

    private static LogicalSchemaNode MergeNodes(LogicalSchemaNode? a, LogicalSchemaNode? b)
    {
        // --- null safety ---
        if (a is null && b is null)
            return Object();

        if (a is null)
            return Clone(b!);

        if (b is null)
            return Clone(a);

        // --- Primitive vs Primitive → آخرین غلبه می‌کند ---
        if (a.Kind == NodeKind.Primitive &&
            b.Kind == NodeKind.Primitive)
            return Clone(b);

        // --- Primitive vs non-primitive → ساختار غنی‌تر را نگه داریم ---
        if (a.Kind == NodeKind.Primitive &&
            b.Kind != NodeKind.Primitive)
            return Clone(b);

        if (b.Kind == NodeKind.Primitive &&
            a.Kind != NodeKind.Primitive)
            return Clone(a);

        // --- Array vs Array → merge item ---
        if (a.Kind == NodeKind.Array &&
            b.Kind == NodeKind.Array)
        {
            return Array(
                MergeNodes(a.Item, b.Item)
            );
        }

        // --- Object vs Object → deep merge properties ---
        if (a.Kind == NodeKind.Object &&
            b.Kind == NodeKind.Object)
        {
            var result = Object();

            foreach (var (key, valA) in a.Properties)
                result.Properties[key] = Clone(valA);

            foreach (var (key, valB) in b.Properties)
            {
                if (result.Properties.TryGetValue(key, out var existing))
                    result.Properties[key] = MergeNodes(existing, valB);
                else
                    result.Properties[key] = Clone(valB);
            }

            return result;
        }

        // --- Array vs Object → Array of merged item ---
        if (a.Kind == NodeKind.Array &&
            b.Kind == NodeKind.Object)
        {
            return Array(
                MergeNodes(a.Item, b)
            );
        }

        // --- Object vs Array → Array of merged item ---
        if (a.Kind == NodeKind.Object &&
            b.Kind == NodeKind.Array)
        {
            return Array(
                MergeNodes(a, b.Item)
            );
        }

        // هر چیز عجیب دیگری → آخرین را نگه دار
        return Clone(b);
    }

    private static LogicalSchemaNode Clone(LogicalSchemaNode n)
    {
        return n.Kind switch
        {
            NodeKind.Primitive =>
                new LogicalSchemaNode { Kind = NodeKind.Primitive, PrimitiveType = n.PrimitiveType },

            NodeKind.Array =>
                new LogicalSchemaNode { Kind = NodeKind.Array, Item = Clone(n.Item) },

            NodeKind.Object =>
                new LogicalSchemaNode
                {
                    Kind = NodeKind.Object,
                    Properties = n.Properties.ToDictionary(k => k.Key, v => Clone(v.Value))
                },

            _ => throw new Exception("Invalid SchemaNode")
        };
    }
}