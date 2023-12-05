using System.Text.RegularExpressions;
using Prolexy.Compiler.Ast;

namespace Prolexy.Compiler.Implementations;

public class Lexer
{
    private static readonly List<TokenDefinition> _tokenDefinitions;

    static Lexer()
    {
        _tokenDefinitions = new List<TokenDefinition>
        {
            new(TokenType.Const, "^null", "object`"),
            new(TokenType.Const, "^\\d{4}/\\d{2}/\\d{2}", "datetime"),
            new(TokenType.Const, "^'[^']*'", "string"),
            new(TokenType.Const, "^\"[^\"]*\"", "string"),
            new(TokenType.Const, "^\\d+(\\.\\d+)?", "number"),
            new(TokenType.Const, "^(true|false)", "boolean"),
            new(TokenType.Operation, $"^\\{Operations.BeginParenthesis}"),
            new(TokenType.Operation, $"^\\{Operations.EndParenthesis}"),
            new(TokenType.Operation, $"^\\{Operations.Comma}"),
            new(TokenType.Operation, $"^\\{Operations.Point}"),
            new(TokenType.Operation, $"^\\{Operations.ArrowFunction}"),
        };
        foreach (var key in Keywords.AllKeywords)
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Keyword, $"^{key}"));
        foreach (var op in Operations.StringOperations
                     .Union(Operations.LogicalOperations)
                     .Union(Operations.DateOperations)
                     .Union(Operations.RelationalOperations))
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Operation, $"^{op}"));
        foreach (var op in Operations.NumericOperations)
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Operation, $"^\\{op}"));
        _tokenDefinitions.Add(new(TokenType.Identifier, "^\\w(\\w|\\d|_)*", "number"));
        _tokenDefinitions.Add(new (TokenType.Const, @"^\$\{\w+\:[\u0600-\u06FF,\w,\s]*\:enum\}*", "enum"));
        _tokenDefinitions.Add(new (TokenType.Const, @"^\$\{\w+\:[\u0600-\u06FF,\w,\s]*\:string\}*", "string"));
        _tokenDefinitions.Add(new (TokenType.Const, @"^\$\{\d+\:[\u0600-\u06FF,\w,\s]*\:number\}*", "number"));
    }

    public Token[] Tokenize(string lqlText)
    {
        var tokens = new List<Token>();

        string? remainingText = lqlText;

        while (!string.IsNullOrWhiteSpace(remainingText))
        {
            var match = FindMatch(remainingText);
            if (match.IsMatch)
            {
                tokens.Add(new Token(match.TokenType, match.Value, match.Type));
                remainingText = match.RemainingText;
            }
            else
            {
                if (IsWhitespace(remainingText))
                {
                    remainingText = remainingText.Substring(1);
                }
                else
                {
                    if(remainingText == "$") break;
                    var invalidTokenMatch = CreateInvalidTokenMatch(remainingText);
                    tokens.Add(new Token(invalidTokenMatch.TokenType, invalidTokenMatch.Value));
                    remainingText = invalidTokenMatch.RemainingText;
                }
            }
        }

        tokens.Add(new Token(TokenType.Eof, string.Empty));

        return tokens.ToArray();
    }

    private TokenMatch FindMatch(string lqlText)
    {
        var result = new TokenMatch() { IsMatch = false };
        foreach (var tokenDefinition in _tokenDefinitions)
        {
            var match = tokenDefinition.Match(lqlText);
            if (match.IsMatch)
            {
                result = match;
                if ((match.TokenType != TokenType.Keyword && match.TokenType != TokenType.Operation) || match.RemainingText?.Length == 0 ||
                    !char.IsAsciiLetterOrDigit(match.RemainingText![0]))
                    return match;
            }
        }

        return result;
    }

    private bool IsWhitespace(string lqlText)
    {
        return Regex.IsMatch(lqlText, "^\\s+");
    }

    private TokenMatch CreateInvalidTokenMatch(string lqlText)
    {
        // var match = Regex.Match(lqlText, "(^\\S+\\s)|^\\S+");
        // if (match.Success)
        // {
        //     return new TokenMatch()
        //     {
        //         IsMatch = true,
        //         RemainingText = lqlText.Substring(match.Length),
        //         TokenType = TokenType.Invalid,
        //         Value = match.Value.Trim()
        //     };
        // }

        throw new Exception($"Failed to generate invalid token: '{lqlText}'");
    }

    private Token ScanKeyword(string keyword)
    {
        return new(TokenType.Keyword, keyword);
    }

    private Token ScanOperator(string op)
    {
        return new(TokenType.Operation, op);
    }

    public class TokenDefinition
    {
        private Regex _regex;
        private readonly TokenType _returnsToken;
        private readonly string? _type;

        public TokenDefinition(TokenType returnsToken, string regexPattern)
        {
            _regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
            _returnsToken = returnsToken;
        }

        public TokenDefinition(TokenType returnsToken, string regexPattern, string type)
        {
            _regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
            _returnsToken = returnsToken;
            _type = type;
        }

        public TokenMatch Match(string inputString)
        {
            var match = _regex.Match(inputString);
            if (match.Success)
            {
                string remainingText = string.Empty;
                if (match.Length != inputString.Length)
                    remainingText = inputString.Substring(match.Length);

                return new TokenMatch()
                {
                    IsMatch = true,
                    RemainingText = remainingText,
                    TokenType = _returnsToken,
                    Value = match.Value,
                    Type = _type
                };
            }

            return new TokenMatch() { IsMatch = false };
        }
    }

    public class TokenMatch
    {
        public bool IsMatch { get; set; }
        public TokenType TokenType { get; set; }
        public string Value { get; set; } = "";
        public string? RemainingText { get; set; }
        public string? Type { get; set; }
    }
}