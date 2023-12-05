namespace Prolexy.Compiler.Implementations;

public record Span(int Start, int End)
{
    public int Start { get; set; } = Start;
    public int End { get; set; } = End;

    public bool Contains(int index)
    {
        return Start <= index && index <= End;
    }
}