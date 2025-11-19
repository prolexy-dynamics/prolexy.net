using Newtonsoft.Json.Linq;

namespace Prolexy.Compiler.Tests.Evaluator;

public class MyBusinessObject
{
    public DateTime OrderDate { get; set; }
    public string name { get; set; }
    public string family { get; set; }
    public string fullname { get; set; }
    public decimal age { get; set; }
    public MyBusinessObject brother { get; set; }
    public string CouponKey { get; set; }
    public int TotalOrderPrice { get; set; }
    public int DiscountPercentage { get; set; }
    public Array LineItems { get; set; }
    public int DamageUnUseHistory { get; set; }
    public JObject AdditionalData { get; set; } = new()
    {
        ["brotherName"] = "Alex",
        ["father"] = new JObject()
        {
            ["name"] = "Joe",
            ["incomes"] = new JArray(){10, 20}
        } 
    };
}