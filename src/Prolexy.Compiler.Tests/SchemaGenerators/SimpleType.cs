using Tiba.Domain.Model.Uoms;

namespace Prolexy.Compiler.Tests.SchemaGenerators;

public class SimpleType
{
    public string Name { get; set; }
    public int Age { get; set; }
    public DateTime RegistrationDate { get; set; }
    public bool Accepted { get; set; }
}
public class SimpleTypeWithArray
{
    public string[] Name { get; set; }
    public int[] Age { get; set; }
    public DateTime[] RegistrationDate { get; set; }
    public bool[] Accepted { get; set; }
}
public class SimpleTypeWithMethod
{
    public string Name { get; set; }
    public int Age { get; set; }
    public DateTime GetBirthDay() => DateTime.Now;
    public void Register(string reason){}
}