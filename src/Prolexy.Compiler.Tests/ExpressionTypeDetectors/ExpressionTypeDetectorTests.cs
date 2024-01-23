using Prolexy.Compiler.Tests.ParserTests;
using TestStack.BDDfy;
using Tiba.Domain.Model.Uoms;

namespace Prolexy.Compiler.Tests.ExpressionTypeDetectors;

[Story(Title = "Detect return type of expression", AsA = "Programmer",
    IWant = "to evaluate what type is returned by the valid expression.")]
public class ExpressionTypeDetectorTests
{
    [Fact]
    public void Should_can_detect_type_of_literal()
    {
        new Should_can_evaluate_expression_return_type()
            .WithExamples(new ExampleTable("expression", "context", "expectedType")
            {
                { "'yaser'", new { }, typeof(string) },
                { "true", new { }, typeof(bool) },
                { "false", new { }, typeof(bool) },
                { "10", new { }, typeof(decimal) },
                { "2020/10/12", new { }, typeof(DateTime) },
            })
            .BDDfy<ExpressionTypeDetectorTests>();
    }
    [Fact]
    public void Should_can_detect_type_of_Expression_with_priority_operation()
    {
        new Should_can_evaluate_expression_return_type()
            .WithExamples(new ExampleTable("expression", "context", "expectedType")
            {
                { "('yaser' + ' ') + 'abbasi'", new { }, typeof(string) },
                { "(true)", new { }, typeof(bool) },
                { "(10)", new { }, typeof(decimal) },
                { "(2020/10/12)", new { }, typeof(DateTime) },
            })
            .BDDfy<ExpressionTypeDetectorTests>();
    }

    [Fact]
    public void Should_can_detect_type_of_implicit_access_member()
    {
        var context = new { name = "yasser", age = 40, birthDay = new DateTime(1983, 10, 10), married = true };
        new Should_can_evaluate_expression_return_type()
            .WithExamples(new ExampleTable("expression", "context", "expectedType")
            {
                { "name", context, typeof(string) },
                { "married", context, typeof(bool) },
                { "age", context, typeof(int) },
                { "birthDay", context, typeof(DateTime) },
            })
            .BDDfy<ExpressionTypeDetectorTests>();
    }

    [Fact]
    public void Should_can_detect_type_of_inner_access_member()
    {
        var context = new
        {
            person =
                new { name = "yasser", age = 40, birthDay = new DateTime(1983, 10, 10), married = true }
        };
        new Should_can_evaluate_expression_return_type()
            .WithExamples(new ExampleTable("expression", "context", "expectedType")
            {
                { "person.name", context, typeof(string) },
                { "person.married", context, typeof(bool) },
                { "person.age", context, typeof(int) },
                { "person.birthDay", context, typeof(DateTime) },
            })
            .BDDfy<ExpressionTypeDetectorTests>();
    }

    [Fact]
    public void Should_can_detect_type_of_method_result()
    {
        var context = new Person();
        new Should_can_evaluate_expression_return_type()
            .WithExamples(new ExampleTable("expression", "context", "expectedType")
            {
                { "Method1()", context, typeof(Int32) },
                { "Method2()", context, typeof(string) },
                { "Method1(1)", context, typeof(string) },
                { "Method2(2)", context, typeof(Int32) },
            })
            .BDDfy<ExpressionTypeDetectorTests>();
    }

    [Fact]
    public void Should_can_detect_type_of_extension_methods_result()
    {
        var context = new Person();
        new Should_can_evaluate_expression_return_type()
            .WithExamples(new ExampleTable("expression", "context", "expectedType")
            {
                { "Grades.Sum(def x => x)", context, typeof(decimal) },
                { "Grades.Aggregate(0, def x, y => x + y)", context, typeof(decimal) },
                { "Grades.Aggregate(new MoneyData(), def x, y => x + y)", context, typeof(MoneyData) },
                { "Grades.Single(def x => x > 10)", context, typeof(Int32) },
                { "Grades.First()", context, typeof(Int32) },
            })
            .BDDfy<ExpressionTypeDetectorTests>();
    }

    [Fact]
    public void Should_can_detect_type_of_instantiation()
    {
        var context = new Person();
        new Should_can_evaluate_expression_return_type()
            .WithExamples(new ExampleTable("expression", "context", "expectedType")
            {
                { "new MoneyData()", context, typeof(MoneyData) },
            })
            .BDDfy<ExpressionTypeDetectorTests>();
    }

    [Fact]
    public void Should_can_detect_type_of_numbers_binary_operation_access_member()
    {
        var context = new { age = 40 };
        new Should_can_evaluate_expression_return_type()
            .WithExamples(new ExampleTable("expression", "context", "expectedType")
            {
                { "age == 10", context, typeof(bool) },
                { "age != 10", context, typeof(bool) },
                { "age > 10", context, typeof(bool) },
                { "age >= 10", context, typeof(bool) },
                { "age < 10", context, typeof(bool) },
                { "age <= 10", context, typeof(bool) },

                { "age + 10", context, typeof(decimal) },
                { "age - 10", context, typeof(decimal) },
                { "age * 10", context, typeof(decimal) },
                { "age / 10", context, typeof(decimal) },
            })
            .BDDfy<ExpressionTypeDetectorTests>();
    }

    [Fact]
    public void Should_can_detect_type_of_string_binary_operation_access_member()
    {
        var context = new { name = "Yasser" };
        new Should_can_evaluate_expression_return_type()
            .WithExamples(new ExampleTable("expression", "context", "expectedType")
            {
                { "name is ''", context, typeof(bool) },
                { "name is not ''", context, typeof(bool) },

                { "name + ' Abbasi'", context, typeof(string) },
                { "10 + ' Abbasi'", context, typeof(string) },
            })
            .BDDfy<ExpressionTypeDetectorTests>();
    }

    [Fact]
    public void Should_can_detect_type_of_datetime_binary_operation_access_member()
    {
        var context = new { birthDay = DateTime.Parse("1983-10-10") };
        new Should_can_evaluate_expression_return_type()
            .WithExamples(new ExampleTable("expression", "context", "expectedType")
            {
                { "birthDay is 1983-10-10", context, typeof(bool) },
                { "birthDay is not 1983-10-10", context, typeof(bool) },
                { "birthDay before 1983-10-10", context, typeof(bool) },
                { "birthDay before or equal to 1983-10-10", context, typeof(bool) },
                { "birthDay after 1983-10-10", context, typeof(bool) },
                { "birthDay after or equal to 1983-10-10", context, typeof(bool) },
            })
            .BDDfy<ExpressionTypeDetectorTests>();
    }
}

public class Person
{
    public int[] Grades { get; set; }
    public int Method1() => 1;
    public string Method2() => "1";
    public string Method1(int input) => "1";
    public int Method2(int input) => 1;
}