namespace Prolexy.Compiler.Tests.ExpressionTypeDetectors;

public class Person
{
    public int[] Grades { get; set; }
    public string BankBranchCode { get; set; }
    public int Method1() => 1;
    public string Method2() => "1";
    public string Method1(int input) => "1";
    public int Method2(int input) => 1;
}