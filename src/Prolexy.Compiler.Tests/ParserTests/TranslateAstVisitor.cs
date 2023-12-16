using Prolexy.Compiler.Ast;
using Prolexy.Compiler.Models;

namespace Prolexy.Compiler.Tests.ParserTests;

public class TranslateAstVisitor : IAstVisitor<FormatterContext, string>
{
    public string VisitBinary(Binary binary, FormatterContext context)
    {
        return $"{binary.Left.Visit(this, context)} {binary.Operation} {binary.Right.Visit(this, context)}";
    }

    public string VisitAssignment(Assignment assignment, FormatterContext context)
    {
        return $"{assignment.Left.Visit(this, context)} = {assignment.Right.Visit(this, context)}";
    }

    public string VisitAccessMember(AccessMember accessMember, FormatterContext context)
    {
        return $"{accessMember.Left.Visit(this, context)}.{accessMember.Token.Value}";
    }

    public string VisitImplicitAccessMember(ImplicitAccessMember implicitAccessMember, FormatterContext context)
    {
        return $"{implicitAccessMember.Token.Value}";
    }

    public string VisitLiteral(LiteralPrimitive literalPrimitive, FormatterContext context)
    {
        return literalPrimitive.Token.Type switch
        {
            "string" => $"\"{literalPrimitive.Token.Value?.Substring(1, literalPrimitive.Token.Value.Length - 2)}\"",
            "datetime" => $"new Date(\"{literalPrimitive.Token.Value}\")",
            _ => literalPrimitive.Token.Value!
        };
    }

    public string VisitStatements(Statement statement, FormatterContext context)
    {
        return statement.Statements
            .Aggregate("",
                (a, b) => a + b.Visit(this, context)?.ToString()!.Insert(0, "".PadLeft(context.Indent * 4, ' ')) +
                          (b is not IfStatement ? ";\n" : ""));
    }

    public string VisitIfStatement(IfStatement ifStatement, FormatterContext context)
    {
        var result = $"if({ifStatement.Condition.Visit(this, context)}){{\n" +
                     $"{ifStatement.ThenStatement.Visit(this, new FormatterContext(Indent: context.Indent + 1))}}}";
        if (ifStatement.ElseStatement.Statements.Any())
            result +=
                $" else {{\n{ifStatement.ElseStatement.Visit(this, new FormatterContext(Indent: context.Indent + 1))}}}";
        return result;
    }

    public string VisitMethodCall(Call call, FormatterContext context)
    {
        var args = string.Join(", ", call.Arguments.Select(a => a.Visit(this, context)?.ToString()));
        return $"{call.MethodSelector.Visit(this, context)}({args})";
    }

    public string VisitPriority(Priority priority, FormatterContext context)
    {
        return $"({priority.InnerAst.Visit(this, context)})";
    }

    public string VisitAnonymousMethod(AnonymousMethod anonymousMethod, FormatterContext context)
    {
        return $"({string.Join(",", anonymousMethod.Parameters.Select(p => p.Value))}) => {anonymousMethod.Expression.Visit(this, context)}";
    }

    public string VisitInstantiation(Instantiation instantiation, FormatterContext context)
    {
        var args = string.Join(", ", instantiation.Arguments.Select(a => a.Visit(this, context)?.ToString()));
        return $"new {instantiation.Typename}({args})";
    }
}