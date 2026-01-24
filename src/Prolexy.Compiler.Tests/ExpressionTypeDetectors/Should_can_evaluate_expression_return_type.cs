using System.Dynamic;
using FluentAssertions;
using Prolexy.Compiler.Ast;
using Prolexy.Compiler.ExtensionMethods;
using Prolexy.Compiler.ExtensionMethods.Enumerable;
using Prolexy.Compiler.Models;
using Prolexy.Compiler.Visitors.TypeDetectorVisitors;
using Tiba.Domain.Model.Uoms;
using Tiba.PortfolioManagement.Domain.Contracts.Models.MoneyAssetAccounts;

namespace Prolexy.Compiler.Tests.ExpressionTypeDetectors;

public class Should_can_evaluate_expression_return_type
{
    private TypeDetectorResult _result = null!;
    readonly ICompiler _compiler = new Implementations.Compiler();
    private string _expression;

    void GivenIEnterExpression(string expression)
    {
        _expression = expression;
    }

    void WhenIWantEvaluateReturnExpressionOnTheContext(object context)
    {
        var evalContext = EvaluatorContextBuilder
            .Default
            .AsClrEvaluatorBuilder()
            .AddClrType<MoneyData>()
            //.WithExtensionMethod(new GetPersonalGlAccountsMethod())
            .AsExpressionTypeDetectorContextBuilder()
            .WithBusinessObjectType(context.GetType())
            .Build();
        var evaluator = _compiler.CompileExpression(_expression).AsExpressionClrReturnTypeEvaluator();
        _result = evaluator.Evaluate(evalContext);
    }

    void ThenIClrTypeAsAnExpected(Type expectedType)
    {
        _result.Result.Should().NotBeNull();
        var nullable = _result.Result!.Name.Contains("Nullable") && expectedType.Name.Contains("Nullable");
        if (!nullable)
            _result.Result.Should().Be(expectedType);
    }
}
public record GetPersonalGlAccountsMethod() :
    Method("GetPersonalGlAccounts", PrimitiveType.Void,
    [
        new Parameter("branchCode", PrimitiveType.String)
    ], new ClrType(typeof(BranchGlAccount[])))
{
    public override object Eval(IEvaluatorVisitor visitor, IEvaluatorContext context, object methodContext,
        IEnumerable<IAst> args)
    {
        var argList = args as IList<IAst> ?? args.ToList();
        dynamic parameters = new ExpandoObject();
        foreach (var parameter in Parameters)
        {
            var value = visitor.Visit(argList[0], context).Value;

            parameters[parameter.ParameterName] = value;
        }

        var branchCode = (string)parameters["branchCode"];

        return new[]
        {
            new BranchGlAccount
            {
                Code = "1094005010502000000000000000",
                Title = "null",
                BranchCode = branchCode,
                NormalBalance = NormalBalance.Credit,
                Balance = new MoneyData(100.00m, "USD")
            }
        };
    }

    public override bool Accept(object value, bool implicitAccessMethod)
    {
        return implicitAccessMethod;
    }
}

public class BranchGlAccount
{
    public required string Code { get; set; }
    public required string Title { get; set; }
    public required string BranchCode { get; set; }
    public NormalBalance NormalBalance { get; set; }
    public required MoneyData Balance { get; set; }

    public NormalBalance OppositeNormalBalance() =>
        NormalBalance == NormalBalance.Debit ? NormalBalance.Credit : NormalBalance.Debit;
}