using Prolexy.Compiler.Ast;

namespace Prolexy.Compiler.Implementations;

public class Parser : IParser
{
    private Token[] _tokens = Array.Empty<Token>();
    private readonly Lexer _lexer = new();
    private int _index;
    private readonly Stack<Span> _spans = new();

    public IAst Parse(string input)
    {
        var tokens = _lexer.Tokenize(input);
        return Parse(tokens);
    }

    public IAst Parse(Token[] tokens)
    {
        _tokens = tokens;
        _index = 0;
        return ParseStatements();
    }

    public IAst ParseExpression(string input)
    {
        _tokens = _lexer.Tokenize(input);
        _index = 0;
        return ParseExp();
    }

    public IAst ParseExpression(Token[] tokens)
    {
        _tokens = tokens;
        _index = 0;
        return ParseExp();
    }

    private Token Peek()
    {
        return _tokens[_index];
    }

    private void Advance()
    {
        _index++;
    }

    void StartSpan()
    {
        _spans.Push(new Span(_index, _index));
    }

    Span CloseSpan()
    {
        var span = _spans.Pop()!;
        span.End = _index;
        return span;
    }

    Span CloneSpan()
    {
        var span = _spans.Peek() with { End = _index };
        span.End = _index;
        return span;
    }

    private IAst ParseStatements()
    {
        StartSpan();
        var results = new List<IAst>();
        do
        {
            if (Peek().TokenType == TokenType.Eof)
                break;
            results.Add(ParseStatement());
        } while (ConsumeOptional(Keywords.AndThen, TokenType.Keyword) != null);

        return new Statement(results, CloseSpan());
    }

    private IAst ParseStatement()
    {
        StartSpan();
        var head = Peek();
        if (head.TokenType != TokenType.Keyword)
            throw new ExpectedKeywords(new[] { Keywords.Set, Keywords.Call, Keywords.If }, _index);
        IAst result;
        switch (head.Value)
        {
            case Keywords.Set:
                Advance();
                result = ParseAssignment();
                break;
            case Keywords.Call:
                result = ParseCallStatement();
                break;
            case Keywords.If:
                Advance();
                result = ParseIf();
                break;
            default:
                throw new ExpectedKeywords(new[] { Keywords.Set, Keywords.Call, Keywords.If }, _index);
        }

        return result;
    }

    private IAst ParseIf()
    {
        var cond = ParseExp();
        var then = ParseThen();
        var elseStmnts = ParseElse();
        return new IfStatement(cond, then, elseStmnts, CloseSpan());
    }

    Statement ParseThen()
    {
        StartSpan();
        var result = new List<IAst>();
        if (ConsumeOptional(Keywords.Then, TokenType.Keyword) == null)
        {
            ExpectedKeywords(new[] { Keywords.Then });
        }

        do
        {
            result.Add(ParseStatement());
        } while (ConsumeOptional(Keywords.AndThen, TokenType.Keyword) != null);

        return new Statement(result, CloseSpan());
    }

    Statement ParseElse()
    {
        StartSpan();
        var result = new List<IAst>();
        do
        {
            if (ConsumeOptional(Keywords.Else, TokenType.Keyword) == null) break;
            result.Add(ParseStatement());
        } while (ConsumeOptional(Keywords.AndThen, TokenType.Keyword) != null);

        if (ConsumeOptional(Keywords.End, TokenType.Keyword) == null)
            throw new ExpectedKeywords(new[] { Keywords.End }, _index);
        return new Statement(result, CloseSpan());
    }

    IAst ParseAssignment()
    {
        IAst left = ParseChain(), right = null!;
        if (ConsumeOptional(Keywords.With, TokenType.Keyword) != null)
            right = ParseExp();
        else
        {
            ExpectedKeywords(new[] { Keywords.With });
        }

        return new Assignment(left, right, CloseSpan());
    }

    private void ExpectedKeywords(string[] keywords)
    {
        throw new ExpectedKeywords(keywords, _index);
    }

    private IAst ParseExp()
    {
        return ParseLogicalOr();
    }

    private IAst ParseLogicalOr()
    {
        StartSpan();
        var result = ParseLogicalAnd();
        while (ConsumeOptional(Operations.Or) != null)
        {
            var right = ParseLogicalAnd();
            result = new Binary(Operations.Or, result, right, CloseSpan());
            StartSpan();
            ;
        }

        CloseSpan();
        return result;
    }

    private IAst ParseLogicalAnd()
    {
        StartSpan();
        var result = ParseRelational();
        while (ConsumeOptional(Operations.And) != null)
        {
            var right = ParseRelational();
            result = new Binary(Operations.And, result, right, CloseSpan());
            StartSpan();
        }

        CloseSpan();
        return result;
    }

    private IAst ParseRelational()
    {
        StartSpan();
        var result = ParseAdditive();

        var head = Peek();
        while (head?.TokenType == TokenType.Operation)
        {
            if (Operations.RelationalOperations.Contains(head.Value) ||
                Operations.StringOperations.Contains(head.Value) ||
                Operations.DateOperations.Contains(head.Value))
            {
                Advance();
                var right = ParseAdditive();
                result = new Binary(head.Value!, result, right, CloseSpan());
                StartSpan();
                ;
                head = Peek();
                continue;
            }

            break;
        }

        CloseSpan();
        return result;
    }

    private IAst ParseAdditive()
    {
        StartSpan();
        ;
        var result = ParseMultiplitive();
        var head = Peek();
        while (head?.TokenType == TokenType.Operation)
        {
            switch (head.Value)
            {
                case Operations.Plus:
                case Operations.Minus:
                    Advance();
                    var right = ParseMultiplitive();
                    result = new Binary(head.Value!, result, right, CloseSpan());
                    head = Peek();
                    StartSpan();
                    ;
                    continue;
            }

            break;
        }

        CloseSpan();
        return result;
    }

    private IAst ParseMultiplitive()
    {
        StartSpan();
        var result = ParseChain();
        var head = Peek();
        while (head?.TokenType == TokenType.Operation)
        {
            switch (head.Value)
            {
                case Operations.Multiply:
                case Operations.Module:
                case Operations.Power:
                case Operations.Devide:
                    Advance();
                    var right = ParseChain();
                    result = new Binary(head.Value, result, right, CloseSpan());
                    head = Peek();
                    StartSpan();
                    ;
                    continue;
            }

            break;
        }

        CloseSpan();
        return result;
    }

    private IAst ParseChain()
    {
        StartSpan();
        var result = ParsePrimary();
        while (true)
        {
            if (ConsumeOptional(Operations.BeginParenthesis) != null)
            {
                result = ParseCall((IAccessMember)result);
            }
            else if (ConsumeOptional(Operations.Point) != null)
            {
                result = ParseAccessMember(result);
            }
            else
            {
                CloseSpan();
                break;
            }

            StartSpan();
        }

        return result;
    }

    private Token? ConsumeOptional(string op, TokenType tokenType = TokenType.Operation)
    {
        var head = Peek();
        if (head.TokenType == tokenType && head.Value == op)
        {
            Advance();
            return head;
        }

        return null;
    }

    private IAst ParsePrimary()
    {
        StartSpan();
        if (ConsumeOptional(Operations.BeginParenthesis) != null)
        {
            var result = ParseExp();
            if (ConsumeOptional(Operations.EndParenthesis) == null)
                throw ExpectedTokenTypes(new[] { TokenType.Operation }, new[] { Operations.EndParenthesis });
            return new Priority(result, CloseSpan());
        }

        var head = Peek();
        if (head == Token.Eof || head == null)
        {
            CloseSpan();
            throw ExpectedTokenTypes(new[] { TokenType.Const, TokenType.Identifier }, null);
        }

        if (head.TokenType == TokenType.Identifier)
        {
            Advance();
            var result = (IAccessMember)new ImplicitAccessMember(head, CloneSpan());
            while (ConsumeOptional(Operations.Point) is not null)
                result = ParseAccessMember(result);
            CloseSpan();
            return result;
        }

        if (head.TokenType == TokenType.Const)
        {
            Advance();
            return new LiteralPrimitive(head, CloseSpan());
        }

        if (head.TokenType == TokenType.Keyword)
        {
            switch (head.Value)
            {
                case Keywords.False:
                case Keywords.True:
                    Advance();
                    return new LiteralPrimitive(head, CloseSpan());
            }
        }

        CloseSpan();
        throw ExpectedTokenTypes(new[] { TokenType.Const, TokenType.Identifier }, null);
    }

    private Exception ExpectedTokenTypes(TokenType[] p0, string[]? strings = null)
    {
        return new ExpectedTokenTypes(p0, strings);
    }

    private IAst ParseCallStatement()
    {
        Advance();
        var left = ParsePrimary();
        if (left is not IAccessMember)
            throw ExpectedTokenTypes(new[] { TokenType.Identifier }, null);

        var head = Peek();
        if (head?.Value != Operations.BeginParenthesis)
            throw ExpectedTokenTypes(new[] { TokenType.Operation }, new[] { Operations.BeginParenthesis });
        Advance();
        var args = ParseArgs();
        if (ConsumeOptional(Operations.EndParenthesis) == null)
        {
            throw ExpectedTokenTypes(new[] { TokenType.Operation }, new[] { Operations.EndParenthesis });
        }

        return new Call((IAccessMember)left, args, CloseSpan());
    }

    private IAst ParseCall(IAccessMember left)
    {
        var args = ParseArgs();
        if (ConsumeOptional(Operations.EndParenthesis) != null)
            return new Call(left, args, CloseSpan());
        throw ExpectedTokenTypes(new[] { TokenType.Operation }, new[] { Operations.EndParenthesis });
    }

    private IAccessMember ParseAccessMember(IAst left)
    {
        var token = Peek();
        if (token.TokenType == TokenType.Identifier)
        {
            Advance();
            return new AccessMember(left, token, CloneSpan());
        }

        Advance();
        return new AccessMember(left, token, CloneSpan());
    }

    private List<IAst> ParseArgs()
    {
        var args = new List<IAst>();
        var head = Peek();
        while (true)
        {
            if (head == null)
                throw ExpectedTokenTypes(new[] { TokenType.Const, TokenType.Identifier },
                    new[] { Operations.EndParenthesis });
            if (head.TokenType == TokenType.Operation && head.Value == Operations.EndParenthesis)
                break;
            next:
            if (ConsumeOptional(Keywords.Def, TokenType.Keyword) is not null)
                args.Add(ParseAnonymousMethod());
            else
                args.Add(ParseExp());
            if (ConsumeOptional(Operations.Comma) != null) goto next;
            head = Peek();
        }

        return args;
    }

    private AnonymousMethod ParseAnonymousMethod()
    {
        StartSpan();
        var head = Peek();
        List<Token> parameters = new();
        while (head.TokenType == TokenType.Identifier)
        {
            parameters.Add(head);
            Advance();
            if (ConsumeOptional(Operations.Point) is null)
                break;
            head = Peek();
        }

        if (ConsumeOptional(Operations.ArrowFunction) is null)
            throw ExpectedTokenTypes(new[] { TokenType.Operation }, new[] { Operations.ArrowFunction });
        var body = ParseExp();
        return new AnonymousMethod(parameters, body, CloseSpan());
    }
}