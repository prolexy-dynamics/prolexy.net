namespace Prolexy.Compiler.Implementations;

public static class Operations
{
    public const string StartsWith = "starts with";
    public const string EndsWith = "ends with";
    public const string Contains = "contains";
    public const string NotStartsWith = "not starts with";
    public const string NotEndsWith = "not ends with";
    public const string NotContains = "not contains";
    public const string Empty = "empty";
    public const string NotEmpty = "notempty";
    public const string After = "after";
    public const string AfterOrEq = "after or equal to";
    public const string Before = "before";
    public const string BeforeOrEq = "before or equal to";
    public const string Plus = "+";
    public const string Minus = "-";
    public const string Multiply = "*";
    public const string Devide = "/";
    public const string Module = "%";
    public const string Power = "^";
    public const string BeginParenthesis = "(";
    public const string EndParenthesis = ")";
    public const string IsNot = "is not";
    public const string Is = "is";
    public const string Eq = "==";
    public const string Neq = "!=";
    public const string Lte = "<=";
    public const string Lt = "<";
    public const string Gte = ">=";
    public const string Gt = ">";
    public const string Or = "or";
    public const string And = "and";
    public const string Point = ".";
    public const string ArrowFunction = "=>";
    public const string Comma = ",";

    public static readonly string[] LogicalOperations = new[] { And, Or };

    public static readonly string[] RelationalOperations = new[]
    {
        IsNot, Is, Neq, Eq, Lte, Lt, Gte, Gt
    };

    public static readonly string[] NumericOperations = new[]
        { Plus, Minus, Multiply, Devide, Power };

    public static readonly string[] StringOperations = new[]
    {
        Contains, NotContains, StartsWith, NotStartsWith,
        EndsWith, NotEndsWith, Empty, NotEmpty
    };

    public static readonly string[] DateOperations = new[]
        { AfterOrEq, After, BeforeOrEq, Before };

    public static readonly string[] Binaryoperations =
        DateOperations.Union(LogicalOperations)
            .Union(RelationalOperations)
            .Union(NumericOperations)
            .Union(StringOperations).ToArray();
}