using Prolexy.Compiler.Implementations;

namespace Prolexy.Compiler.Ast;

using System.Text.RegularExpressions;

public readonly record struct TextPosition(int Line, int Column, int Index);
public readonly record struct TextSpan(TextPosition Start, TextPosition End);

public record Token(TokenType TokenType, string? Value = null, string? Type = null, TextSpan Span = default)
{
    public static Token Eof { get; } = new Token(TokenType.Eof);
}

public class Lexer
{
    private static readonly List<TokenDefinition> _tokenDefinitions;

    static Lexer()
    {
        _tokenDefinitions = new List<TokenDefinition>
        {
            new(TokenType.Const, "^null", "object"),
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
        _tokenDefinitions.Add(new(TokenType.Const, @"^\$\{\w+\:[\u0600-\u06FF,\w,\s]*\:enum\}*", "enum"));
        _tokenDefinitions.Add(new(TokenType.Const, @"^\$\{\w+\:[\u0600-\u06FF,\w,\s]*\:string\}*", "string"));
        _tokenDefinitions.Add(new(TokenType.Const, @"^\$\{\d+\:[\u0600-\u06FF,\w,\s]*\:number\}*", "number"));
    }

    public Token[] Tokenize(string lqlText)
    {
        var tokens = new List<Token>();

        int index = 0;
        int line = 1;
        int column = 1;

        while (index < lqlText.Length)
        {
            string remainingText = lqlText.Substring(index);

            // اگر دقیقاً یک "$" آخر متن دارید (مثل کد فعلی‌تون)
            if (remainingText == "$")
                break;

            // whitespace: یکجا consume کن تا line/column درست جلو بره
            var ws = Regex.Match(remainingText, @"^\s+");
            if (ws.Success)
            {
                AdvancePosition(ws.Value, ref line, ref column);
                index += ws.Length;
                continue;
            }

            var match = FindMatch(remainingText);
            if (match.IsMatch)
            {
                var start = new TextPosition(line, column, index);

                string consumed = match.Value;
                index += consumed.Length;
                AdvancePosition(consumed, ref line, ref column);

                var end = new TextPosition(line, column, index);
                tokens.Add(new Token(match.TokenType, match.Value, match.Type, new TextSpan(start, end)));
            }
            else
            {
                // invalid token (تا قبل از whitespace)
                var invalid = CreateInvalidTokenMatch(remainingText);

                var start = new TextPosition(line, column, index);

                string consumed = invalid.Value;
                index += consumed.Length;
                AdvancePosition(consumed, ref line, ref column);

                var end = new TextPosition(line, column, index);
                tokens.Add(new Token(invalid.TokenType, invalid.Value, Span: new TextSpan(start, end)));
            }
        }

        var eofPos = new TextPosition(line, column, index);
        tokens.Add(new Token(TokenType.Eof, string.Empty, Span: new TextSpan(eofPos, eofPos)));

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

                if ((match.TokenType != TokenType.Keyword && match.TokenType != TokenType.Operation)
                    || match.RemainingText?.Length == 0
                    || !IsAsciiLetterOrDigit(match.RemainingText![0]))
                    return match;
            }
        }

        return result;
    }

    // آپدیت line/column با توجه به متن مصرف‌شده
    private static void AdvancePosition(string consumed, ref int line, ref int column)
    {
        for (int i = 0; i < consumed.Length; i++)
        {
            char c = consumed[i];

            if (c == '\r')
            {
                // CRLF را یک newline حساب کن
                if (i + 1 < consumed.Length && consumed[i + 1] == '\n')
                    i++;

                line++;
                column = 1;
            }
            else if (c == '\n')
            {
                line++;
                column = 1;
            }
            else
            {
                column++;
            }
        }
    }

    private TokenMatch CreateInvalidTokenMatch(string lqlText)
    {
        // هرچیزی تا قبل از whitespace را invalid بگیر
        var match = Regex.Match(lqlText, @"^\S+");
        if (match.Success)
        {
            string remaining = match.Length < lqlText.Length ? lqlText.Substring(match.Length) : string.Empty;

            return new TokenMatch
            {
                IsMatch = true,
                RemainingText = remaining,
                TokenType = TokenType.Invalid,
                Value = match.Value
            };
        }

        // اگر اینجا برسیم یعنی ورودی عجیب بوده (مثلاً رشته خالی یا فقط whitespace که بالاتر handle شده)
        return new TokenMatch { IsMatch = true, RemainingText = "", TokenType = TokenType.Invalid, Value = lqlText[0].ToString() };
    }

    public class TokenDefinition
    {
        private readonly Regex _regex;
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
                string remainingText = match.Length != inputString.Length
                    ? inputString.Substring(match.Length)
                    : string.Empty;

                return new TokenMatch
                {
                    IsMatch = true,
                    RemainingText = remainingText,
                    TokenType = _returnsToken,
                    Value = match.Value,
                    Type = _type
                };
            }

            return new TokenMatch { IsMatch = false };
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

    public static bool IsAsciiLetterOrDigit(char c) => IsAsciiLetter(c) | IsBetween(c, '0', '9');
    public static bool IsAsciiLetter(char c) => (uint)((c | 0x20) - 'a') <= 'z' - 'a';
    public static bool IsBetween(char c, char minInclusive, char maxInclusive) =>
        (uint)(c - minInclusive) <= (uint)(maxInclusive - minInclusive);
}
