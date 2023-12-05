using TestStack.BDDfy;

namespace Prolexy.Compiler.Tests.ParserTests;

[Story(Title = "Parse tokenizes input", AsA = "Programmer",
    IWant = "to parse statement to generate Abstract syntax tree(AST).")]
public class ParserUnitTest
{
    [Fact]
    public void Should_can_parse_literal()
    {
        new Should_can_parse_expression()
            .WithExamples(new ExampleTable("input", "expected syntax")
            {
                { "'yaser'", "\"yaser\"" },
                { "10", "10" },
                { "null", "null" },
                { "true", "true" },
                { "false", "false" },
                { "2020/10/12", "new Date(\"2020/10/12\")" },
            })
            .BDDfy<ParserUnitTest>();
    }
    [Fact]
    public void Should_can_parse_member_access()
    {
        new Should_can_parse_expression()
            .WithExamples(new ExampleTable("input", "expected syntax")
            {
                { "name", "name" },
                { "address.city", "address.city" },
            })
            .BDDfy<ParserUnitTest>();
    }

    [Fact]
    public void Should_can_parse_assignment()
    {
        new Should_can_parse_statements()
            .WithExamples(new ExampleTable("input", "expected syntax")
            {
                { "set name with 'yaser' contains 'hamid'", "name = \"yaser\" contains \"hamid\";" },
                { "set name with 'yaser'", "name = \"yaser\";" },
                { "set age with 20", "age = 20;" },
                { "set age with age + 18", "age = age + 18;" },
            })
            .BDDfy<ParserUnitTest>();
    }

    [Fact]
    public void Should_can_parse_method_call()
    {
        new Should_can_parse_statements()
            .WithExamples(new ExampleTable("input", "expected syntax")
            {
                { "call show()", "show();" },
                { "call show('message')", "show(\"message\");" },
                { "call power(2, 8/2)", "power(2, 8 / 2);" },
                { "call setRegistrationDate(2022/01/12)", "setRegistrationDate(new Date(\"2022/01/12\"));" },
            })
            .BDDfy<ParserUnitTest>();
    }

    [Fact]
    public void Should_can_parse_anonymous_method()
    {
        new Should_can_parse_statements()
            .WithExamples(new ExampleTable("input", "expected syntax")
            {
                { "call cities.Exists(def x => x.name contains 'Teh')", "cities.Exists((x) => x.name contains \"Teh\");" },
                { "call person.show()", "person.show();" },
                { "call show()", "show();" },
                { "call show('message')", "show(\"message\");" },
                { "call power(2, 8/2)", "power(2, 8 / 2);" },
                { "call setRegistrationDate(2022/01/12)", "setRegistrationDate(new Date(\"2022/01/12\"));" },
            })
            .BDDfy<ParserUnitTest>();
    }
    
    [Fact]
    public void Should_can_parse_if_statement()
    {
        new Should_can_parse_statements()
            .WithExamples(new ExampleTable("input", "expected syntax")
            {
                { "if true then set name with 'John' end", "if(true){    name = \"John\";}" },
                { "if power(2, 8/2)< 10 then set name with age *7 and then call show() end", "if(power(2, 8 / 2) < 10){name = age * 7; show();}" },
                { "if power(2, 8/2)< 10 then set name with age *7 end", "if(power(2, 8 / 2) < 10){    name = age * 7;}" },
                { "if age<= 10 then call register(name, age) else call reject() end", "if(age <= 10){ register(name, age);} else {reject();}" },
            })
            .BDDfy<ParserUnitTest>();
    }
}