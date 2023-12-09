using Newtonsoft.Json.Linq;
using TestStack.BDDfy;

namespace Prolexy.Compiler.Tests.Evaluator;

[Story(Title = "Evaluate script", AsA = "Rule manager",
    IWant = "to evaluate statement.",
    SoThat = "I can  execute dynamic rules.")]
public class JsonEvaluatorExpressionUnitTest
{
    [Fact]
    public void Should_can_evaluate_literal()
    {
        new Should_can_evaluate_expression()
            .WithExamples(new ExampleTable("expression", "context", "expected")
            {
                { "'yaser'", "{}", "yaser" },
                { "10", "{}", 10 },
                { "null", "{}", null },
                { "true", "{}", true },
                { "false", "{}", false },
                { "2020/10/12", "{}", new DateTime(2020, 10, 12) },
                { "${Value:Text:string}", "{}", "Value" },
                { "${25:Text:number}", "{}", 25 },
            })
            .BDDfy<JsonEvaluatorExpressionUnitTest>();
    }

    [Fact]
    public void Should_can_evaluate_access_member()
    {
        new Should_can_evaluate_expression()
            .WithExamples(new ExampleTable("expression", "context", "expected")
            {
                // { "name", "{name: 'yasser'}", "yasser" },
                // { "age", "{age:10}", 10 },
                { "nothing", "{}", null },
                { "accepted", "{accepted: true}", true },
                { "birthDay", "{birthDay: '2020-10-12T00:00:00z'}", new DateTime(2020, 10, 12) },
                { "brother.birthDay", "{brother: {birthDay: '2020-10-12T00:00:00z'}}", new DateTime(2020, 10, 12) },
            })
            .BDDfy<JsonEvaluatorExpressionUnitTest>();
    }

    [Fact]
    public void Should_can_evaluate_relational_operation()
    {
        new Should_can_evaluate_expression()
            .WithExamples(new ExampleTable("expression", "context", "expected")
            {
                { "name is null", "{}", true },
                { "name is null", "{name:''}", false },
                { "name is firstName", "{}", true },
                { "name is not null", "{name:''}", true },
                { "name is not null", "{}", false },
                { "name is not firstName", "{}", false },

                { "name is 'yasser'", "{name: 'yasser'}", true },
                { "name is 'yas'", "{name: 'yasser'}", false },
                { "name is not 'yasser'", "{name: 'yasser'}", false },
                { "name is not 'yas'", "{name: 'yasser'}", true },
                { "name contains 'yas'", "{name: 'yasser'}", true },
                { "name contains 'mey'", "{name: 'yasser'}", false },
                { "name starts with 'yas'", "{name: 'yasser'}", true },
                { "name starts with 'er'", "{name: 'yasser'}", false },
                { "name ends with 'er'", "{name: 'yasser'}", true },
                { "name ends with 'yas'", "{name: 'yasser'}", false },

                { "name not contains 'yas'", "{name: 'yasser'}", false },
                { "name not contains 'mey'", "{name: 'yasser'}", true },
                { "name not starts with 'yas'", "{name: 'yasser'}", false },
                { "name not starts with 'er'", "{name: 'yasser'}", true },
                { "name not ends with 'er'", "{name: 'yasser'}", false },
                { "name not ends with 'yas'", "{name: 'yasser'}", true },

                { "age == 10.0", "{age:10}", true },
                { "age == 10.1", "{age:10}", false },
                { "age != 10.0", "{age:10}", false },
                { "age != 10.1", "{age:10}", true },
                { "age < 10.1", "{age:10}", true },
                { "age < 10", "{age:10}", false },
                { "age <= 10", "{age:10}", true },
                { "age > 10", "{age:10}", false },
                { "age >= 10", "{age:10}", true },
                { "age > 9.9", "{age:10}", true },

                { "birthDay is 2020/10/12", "{birthDay: '2020-10-12T00:00:00z'}", true },
                { "birthDay is 2020/10/11", "{birthDay: '2020-10-12T00:00:00z'}", false },
                { "birthDay is not 2020/10/12", "{birthDay: '2020-10-12T00:00:00z'}", false },
                { "birthDay is not 2020/10/11", "{birthDay: '2020-10-12T00:00:00z'}", true },
                { "birthDay after 2020/10/11", "{birthDay: '2020-10-12T00:00:00z'}", true },
                { "birthDay after 2020/10/12", "{birthDay: '2020-10-12T00:00:00z'}", false },
                { "birthDay after or equal to 2020/10/12", "{birthDay: '2020-10-12T00:00:00z'}", true },
                { "birthDay after or equal to 2020/10/13", "{birthDay: '2020-10-12T00:00:00z'}", false },
                { "birthDay before 2020/10/13", "{birthDay: '2020-10-12T00:00:00z'}", true },
                { "birthDay before 2020/10/12", "{birthDay: '2020-10-12T00:00:00z'}", false },
                { "birthDay before or equal to 2020/10/12", "{birthDay: '2020-10-12T00:00:00z'}", true },
                { "birthDay before or equal to 2020/10/11", "{birthDay: '2020-10-12T00:00:00z'}", false },
                { "accepted is true", "{accepted: true}", true },
                { "accepted is not true", "{accepted: true}", false },
            })
            .BDDfy<JsonEvaluatorExpressionUnitTest>();
    }

    [Fact]
    public void Should_can_evaluate_assignment()
    {
        new Should_can_execute()
            .WithExamples(new ExampleTable("rule", "context", "trueExpression")
            {
                {
                    "set fullname with name + ' ' + family", "{name: 'yasser', family: 'abbasi'}",
                    "fullname is 'yasser abbasi'"
                },
                { "set age with age + 0.1 * age", "{age: 20}", "age is 22" },
                { "set brother.age with age + 10", "{brother:{}, age: 10}", "brother.age is 20" },
            })
            .BDDfy<JsonEvaluatorExpressionUnitTest>();
    }

    [Fact]
    public void Should_can_evaluate_if()
    {
        new Should_can_execute()
            .WithExamples(new ExampleTable("rule", "context", "trueExpression")
            {
                {
                    "if(true) then set fullname with name + ' ' + family end", "{name: 'yasser', family: 'abbasi'}",
                    "fullname is 'yasser abbasi'"
                },
                { "if(age != 20) then set age with age + 0.1 * age end", "{age: 20}", "age is 20" },
                {
                    "if(age == 10) then set brother.age with age + 10 end", "{brother:{}, age: 10}", "brother.age is 20"
                },
                {
                    "if(false) then set fullname with name + ' ' + family else set fullname with 'nobody' end",
                    "{name: 'yasser', family: 'abbasi'}",
                    "fullname is 'nobody'"
                },
                {
                    "if(age != 20) then set age with age + 0.1 * age else set age with age + 10 end", "{age: 20}",
                    "age is 30"
                },
                {
                    "if CouponKey contains '2001' and LineItems.Exists(def x => x.Product is 'special product') and TotalOrderPrice > 100000 then set DiscountPercentage with 20 end",
                    @"{
	                ""CouponKey"": ""2001"",
                    ""TotalOrderPrice"": 2000000,
                    ""DiscountPercentage"": 0,
                    ""LineItems"": [{ ""Product"": ""special product"",""Quantity"": 0,""Delivered"": 0  }]
                     }",
                    "DiscountPercentage is 20"
                },
                {
                    "if(age == 10) then set brother.age with age + 10 else set age with age + 10 end",
                    "{brother:{}, age: 10}", "brother.age is 20"
                },
                {
                    "if DamageUnUseHistory == 1 then set DiscountPercentage with DiscountPercentage + 25 else if DamageUnUseHistory == 2 then set DiscountPercentage with DiscountPercentage + 30 else if DamageUnUseHistory == 3 then set DiscountPercentage with DiscountPercentage + 35 else set DiscountPercentage with DiscountPercentage + 40 end end end and then if DiscountPercentage > 70 then set DiscountPercentage with 70 end",
                    "{DiscountPercentage:40, DamageUnUseHistory: 10}", "DiscountPercentage is 70"
                },
            })
            .BDDfy<JsonEvaluatorExpressionUnitTest>();
    }

    [Fact]
    public void Should_can_evaluate_extension_method()
    {
        new Should_can_evaluate_expression()
            .WithExamples(new ExampleTable("expression", "context", "expected")
            {
                { "OrderDate.AddDays(1)", "{OrderDate: '2020-10-12T00:00:00z'}", DateTime.Parse("2020-10-13") },
                { $"Now() after or equal to {DateTime.Now:yyyy/MM/dd}", "{}", true },
                // { "personCities.GroupBy(def x => x.city, def x => x.person).Tehran.Count(def x => true)", 
                //     "{personCities: [{city: 'Tehran', person: 'ali'}]}",
                //     1 },
                { "Cities.Exists(def x => x.name is 'Tehran')", "{Cities: [{name: 'Tehran'}]}", true },
                { "Cities.Exists(def x => x.name is 'Shiraz')", "{Cities: [{name: 'Tehran'}]}", false },
                { "Cities.Count(def x => x.name is 'Tehran')", "{Cities: [{name: 'Tehran'}]}", 1 },
                { "Cities.Count(def x => x.name is 'Shiraz')", "{Cities: [{name: 'Tehran'}]}", 0 },
                { "Grades.Min(def x => x)", "{Grades: [0, 1, 2, 3, 4, 5]}", 0 },
                { "Grades.Max(def x => x)", "{Grades: [0, 1, 2, 3, 4, 5]}", 5 },
                { "Grades.Sum(def x => x)", "{Grades: [0, 1, 2, 3, 4, 5]}", 15 },
                { "Grades.Avg(def x => x)", "{Grades: [0, 1, 2, 3, 4, 5]}", 15 / 6 },
                {
                    "CouponKey contains '2001' and LineItems.Exists(def x => x.Product is 'special product')", 
                    @"{
	                    ""CouponKey"": ""2001"",
                        ""TotalOrderPrice"": 2000000,
                        ""DiscountPercentage"": 0,
                        ""LineItems"": [{ ""Product"": ""special product"",""Quantity"": 0,""Delivered"": 0  }]
                     }",
                    true
                }
            })
            .BDDfy<JsonEvaluatorExpressionUnitTest>();
    }
}