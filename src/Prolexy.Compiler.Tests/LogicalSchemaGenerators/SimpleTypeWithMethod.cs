namespace Prolexy.Compiler.Tests.LogicalSchemaGenerators;

public class SimpleTypeWithMethod
{
    public string Name { get; set; }
    public int Age { get; set; }
    public DateTime GetBirthDay() => DateTime.Now;
    public void Register(string reason){}
}