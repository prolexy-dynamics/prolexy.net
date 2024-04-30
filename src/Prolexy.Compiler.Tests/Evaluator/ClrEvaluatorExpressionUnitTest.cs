using System.Globalization;
using FluentAssertions.Extensions;
using Newtonsoft.Json.Linq;
using TestStack.BDDfy;

namespace Prolexy.Compiler.Tests.Evaluator;

[Story(Title = "Evaluate script", AsA = "Rule manager",
    IWant = "to evaluate statement.",
    SoThat = "I can  execute dynamic rules.")]
public class ClrEvaluatorExpressionUnitTest
{
    [Fact]
    public void Should_can_evaluate_literal()
    {
        new Should_can_evaluate_expression_on_Clr_context()
            .WithExamples(new ExampleTable("expression", "context", "expected")
            {
                { "'yaser'", null, "yaser" },
                { "10", null, 10 },
                { "null", null, null },
                { "true", null, true },
                { "false", null, false },
                { "2020/10/12", null, new DateTime(2020, 10, 12) },
                { "${Value:Text:string}", null, "Value" },
                { "${25:Text:number}", null, 25 },
            })
            .BDDfy<JsonEvaluatorExpressionUnitTest>();
    }

    [Fact]
    public void Should_can_evaluate_access_member()
    {
        new Should_can_evaluate_expression_on_Clr_context()
            .WithExamples(new ExampleTable("expression", "context", "expected")
            {
                { "name", new { name = "yasser" }, "yasser" },
                { "age", new { age = 10 }, 10 },
                { "nothing", new { nothing = (int?)null }, null },
                { "accepted", new { accepted = true }, true },
                { "birthDay", new { birthDay = new DateTime(2020, 10, 12) }, new DateTime(2020, 10, 12) },
                {
                    "brother.birthDay", new { brother = new { birthDay = new DateTime(2020, 10, 12) } },
                    new DateTime(2020, 10, 12)
                },
            })
            .BDDfy<JsonEvaluatorExpressionUnitTest>();
    }

    [Fact]
    public void Should_can_evaluate_relational_operation()
    {
        new Should_can_evaluate_expression_on_Clr_context()
            .WithExamples(new ExampleTable("expression", "context", "expected")
            {
                { "new Person('yasser').Name", new {  }, "yasser" },

                { "name is null", new MyBusinessObject { }, true },
                { "name is null", new MyBusinessObject { name = "" }, false },
                { "name is firstName", new MyBusinessObject { }, true },
                { "name is not null", new MyBusinessObject { name = "" }, true },
                { "name is not null", new MyBusinessObject { }, false },
                { "name is not firstName", new MyBusinessObject { }, false },

                { "name is 'yasser'", new MyBusinessObject { name = "yasser" }, true },
                { "name is 'yas'", new MyBusinessObject { name = "yasser" }, false },
                { "name is not 'yasser'", new MyBusinessObject { name = "yasser" }, false },
                { "name is not 'yas'", new MyBusinessObject { name = "yasser" }, true },
                { "name contains 'yas'", new MyBusinessObject { name = "yasser" }, true },
                { "name contains 'mey'", new MyBusinessObject { name = "yasser" }, false },
                { "name starts with 'yas'", new MyBusinessObject { name = "yasser" }, true },
                { "name starts with 'er'", new MyBusinessObject { name = "yasser" }, false },
                { "name ends with 'er'", new MyBusinessObject { name = "yasser" }, true },
                { "name ends with 'yas'", new MyBusinessObject { name = "yasser" }, false },

                { "name not contains 'yas'", new MyBusinessObject { name = "yasser" }, false },
                { "name not contains 'mey'", new MyBusinessObject { name = "yasser" }, true },
                { "name not starts with 'yas'", new MyBusinessObject { name = "yasser" }, false },
                { "name not starts with 'er'", new MyBusinessObject { name = "yasser" }, true },
                { "name not ends with 'er'", new MyBusinessObject { name = "yasser" }, false },
                { "name not ends with 'yas'", new MyBusinessObject { name = "yasser" }, true },

                { "age == 10.0", new MyBusinessObject { age = 10 }, true },
                { "age == 10.1", new MyBusinessObject { age = 10 }, false },
                { "age != 10.0", new MyBusinessObject { age = 10 }, false },
                { "age != 10.1", new MyBusinessObject { age = 10 }, true },
                { "age < 10.1", new MyBusinessObject { age = 10 }, true },
                { "age < 10", new MyBusinessObject { age = 10 }, false },
                { "age <= 10", new MyBusinessObject { age = 10 }, true },
                { "age > 10", new MyBusinessObject { age = 10 }, false },
                { "age >= 10", new MyBusinessObject { age = 10 }, true },
                { "age > 9.9", new MyBusinessObject { age = 10 }, true },

                { "birthDay is 2020/10/12", new { birthDay = DateTime.Parse("2020-10-12T00:00:00") }, true },
                { "birthDay is 2020/10/11", new { birthDay = DateTime.Parse("2020-10-12T00:00:00") }, false },
                { "birthDay is not 2020/10/12", new { birthDay = DateTime.Parse("2020-10-12T00:00:00") }, false },
                { "birthDay is not 2020/10/11", new { birthDay = DateTime.Parse("2020-10-12T00:00:00") }, true },
                { "birthDay after 2020/10/11", new { birthDay = DateTime.Parse("2020-10-12T00:00:00") }, true },
                { "birthDay after 2020/10/12", new { birthDay = DateTime.Parse("2020-10-12T00:00:00") }, false },
                {
                    "birthDay after or equal to 2020/10/12", new { birthDay = DateTime.Parse("2020-10-12T00:00:00") },
                    true
                },
                {
                    "birthDay after or equal to 2020/10/13", new { birthDay = DateTime.Parse("2020-10-12T00:00:00") },
                    false
                },
                { "birthDay before 2020/10/13", new { birthDay = DateTime.Parse("2020-10-12T00:00:00") }, true },
                { "birthDay before 2020/10/12", new { birthDay = DateTime.Parse("2020-10-12T00:00:00") }, false },
                {
                    "birthDay before or equal to 2020/10/12", new { birthDay = DateTime.Parse("2020-10-12T00:00:00") },
                    true
                },
                {
                    "birthDay before or equal to 2020/10/11", new { birthDay = DateTime.Parse("2020-10-12T00:00:00") },
                    false
                },

                { "accepted is true", new { accepted = true }, true },
                { "accepted is not true", new { accepted = true }, false },
                
            })
            .BDDfy<JsonEvaluatorExpressionUnitTest>();
    }

    [Fact]
    public void Should_can_evaluate_assignment()
    {
        new Should_can_execute_on_clr_context()
            .WithExamples(new ExampleTable("rule", "context", "trueExpression")
            {
                {
                    "set fullname with name + ' ' + family",
                    new MyBusinessObject() { name = "yasser", family = "abbasi", fullname = "" },
                    "fullname is 'yasser abbasi'"
                },
                { "set age with age + 0.1 * age", new MyBusinessObject { age = 20 }, "age is 22" },
                {
                    "set brother.age with age + 10",
                    new MyBusinessObject { brother = new MyBusinessObject { age = 10 }, age = 10 }, "brother.age is 20"
                },
            })
            .BDDfy<JsonEvaluatorExpressionUnitTest>();
    }

    [Fact]
    public void Should_can_evaluate_if()
    {
        new Should_can_execute_on_clr_context()
            .WithExamples(new ExampleTable("rule", "context", "trueExpression")
            {
                {
                    "if(true) then set fullname with name + ' ' + family end",
                    new MyBusinessObject() { name = "yasser", family = "abbasi" },
                    "fullname is 'yasser abbasi'"
                },
                {
                    "if(age != 20) then set age with age + 0.1 * age end", new MyBusinessObject { age = 20 },
                    "age is 20"
                },
                {
                    "if(age == 10) then set brother.age with age + 10 end",
                    new MyBusinessObject() { brother = new MyBusinessObject(), age = 10 },
                    "brother.age is 20"
                },
                {
                    "if(false) then set fullname with name + ' ' + family else set fullname with 'nobody' end",
                    new MyBusinessObject { name = "yasser", family = "abbasi" },
                    "fullname is 'nobody'"
                },
                {
                    "if(age != 20) then set age with age + 0.1 * age else set age with age + 10 end",
                    new MyBusinessObject { age = 20 },
                    "age is 30"
                },
                {
                    "if CouponKey contains '2001' and LineItems.Exists(def x => x.Product is 'special product') and TotalOrderPrice > 100000 then set DiscountPercentage with 20 end",
                    new MyBusinessObject
                    {
                        CouponKey = "2001",
                        TotalOrderPrice = 2000000,
                        DiscountPercentage = 0,
                        LineItems = new[]
                        {
                            new { Product = "special product", Quantity = 0, Delivered = 0 }
                        }
                    },
                    "DiscountPercentage is 20"
                },
                {
                    "if(age == 10) then set brother.age with age + 10 else set age with age + 10 end",
                    new MyBusinessObject { brother = new MyBusinessObject { }, age = 10 }, "brother.age is 20"
                },
                {
                    "if DamageUnUseHistory == 1 then set DiscountPercentage with DiscountPercentage + 25 else if DamageUnUseHistory == 2 then set DiscountPercentage with DiscountPercentage + 30 else if DamageUnUseHistory == 3 then set DiscountPercentage with DiscountPercentage + 35 else set DiscountPercentage with DiscountPercentage + 40 end end end and then if DiscountPercentage > 70 then set DiscountPercentage with 70 end",
                    new MyBusinessObject { DiscountPercentage = 40, DamageUnUseHistory = 10 },
                    "DiscountPercentage is 70"
                },
            })
            .BDDfy<JsonEvaluatorExpressionUnitTest>();
    }

    [Fact]
    public void Should_can_evaluate_extension_method()
    {
        new Should_can_evaluate_expression_on_Clr_context()
            .WithExamples(new ExampleTable("expression", "context", "expected")
            {
                { "'Code,Name'.SplitBy(',').Exists(def a => a is 'Code')", new { BranchCode = "12345" }, true },
                { "'18-' + BranchCode + '-' + Now().Format('yyyyMMdd')", new { BranchCode = "12345" }, $"18-12345-{DateTime.Now.ToString("yyyyMMdd", new CultureInfo("fa"))}"},
                {
                    "OrderDate.AddDays(1)", new MyBusinessObject { OrderDate = new(2020, 10, 12) },
                    DateTime.Parse("2020-10-13")
                },
                { $"Now() after or equal to {DateTime.Now:yyyy/MM/dd}", new MyBusinessObject(), true },
                {
                    "Cities.Exists(def x => x.name is 'Tehran')", new { Cities = new[] { new { name = "Tehran" } } },
                    true
                },
                {
                    "Cities.Exists(def x => x.name is 'Shiraz')", new { Cities = new[] { new { name = "Tehran" } } },
                    false
                },
                { "Cities.Count(def x => x.name is 'Tehran')", new { Cities = new[] { new { name = "Tehran" } } }, 1 },
                { "Cities.Count(def x => x.name is 'Shiraz')", new { Cities = new[] { new { name = "Tehran" } } }, 0 },
                { "Cities.Aggregate('Hi:', def s, i => s + ' ' + i.name)", new { Cities = new[] { new { name = "Tehran" }, new { name = "Shiraz" } } }, "Hi: Tehran Shiraz" },
                { "Grades.Min(def x => x)", new { Grades = Enumerable.Range(0, 6) }, 0 },
                { "Grades.Max(def x => x)", new { Grades = Enumerable.Range(0, 6) }, 5 },
                { "Grades.Sum(def x => x)", new { Grades = Enumerable.Range(0, 6) }, 15 },
                { "Grades.Avg(def x => x)", new { Grades = Enumerable.Range(0, 6) }, 15 / 6 },
                {
                    "CouponKey contains '2001' and LineItems.Exists(def x => x.Product is 'special product')",
                    new
                    {
                        CouponKey = "2001",
                        TotalOrderPrice = 2000000,
                        DiscountPercentage = 0,
                        LineItems = new[]
                        {
                            new { Product = "special product", Quantity = 0, Delivered = 0 }
                        }
                    },
                    true
                },
                { "Iff(Grades.Sum(def x => x) > 10, 'yes', 'no')", new { Grades = Enumerable.Range(0, 6) }, "yes" },
                { "Iff(Grades.Sum(def x => x) > 15, 'yes', 'no')", new { Grades = Enumerable.Range(0, 6) }, "no" },
            })
            .BDDfy<JsonEvaluatorExpressionUnitTest>();
    }

    [Fact]
    public void Should_can_evaluate_expression_on_jobject()
    {
        new Should_can_evaluate_expression_on_Clr_context()
            .WithExamples(new ExampleTable("expression", "context", "expected")
            {
                { "AdditionalData.brotherName.SplitBy('l').Length", new MyBusinessObject { }, 2 },
                { "AdditionalData.brotherName is 'yasser'", new MyBusinessObject { }, false },
                { "'Alex' is AdditionalData.brotherName", new MyBusinessObject { }, true },
                { "new Person(AdditionalData.brotherName).Name", new MyBusinessObject { }, "Alex" },
                { "AdditionalData.brotherName", new MyBusinessObject(), "Alex" },
                { "AdditionalData.father.name", new MyBusinessObject(), "Joe" },
                { "AdditionalData.father.incomes.Sum(def x => x)", new MyBusinessObject(), 30 },
            })
            .BDDfy<JsonEvaluatorExpressionUnitTest>();
    }
}

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