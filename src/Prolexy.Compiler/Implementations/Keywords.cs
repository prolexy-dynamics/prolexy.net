namespace Prolexy.Compiler.Implementations;

public static class Keywords
{
    public const string If = "if";
    public const string Set = "set";
    public const string Call = "call";
    public const string With = "with";
    public const string Else = "else";
    public const string Then = "then";
    public const string AndThen = "and then";
    public const string End = "end";
    public const string Def = "def";
    public const string True = "true";
    public const string False = "false";
    public static string[] AllKeywords = new[] { If, Set, Call, With, Else, Then, AndThen, End, True, False, Def };
}