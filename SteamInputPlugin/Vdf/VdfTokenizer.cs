using System;
using System.Text;

namespace com.github.lhervier.ksp.Vdf
{
    internal enum VdfTokenKind
    {
        End,
        String,
        OpenBrace,
        CloseBrace
    }

    internal sealed class VdfToken
    {
        public VdfTokenKind Kind { get; private set; }
        public string Value { get; private set; }
        public int Line { get; private set; }
        public int Column { get; private set; }

        private VdfToken() { }

        public static VdfToken End()
        {
            return new VdfToken { Kind = VdfTokenKind.End };
        }

        public static VdfToken String(string value, int line, int column)
        {
            return new VdfToken
            {
                Kind = VdfTokenKind.String,
                Value = value,
                Line = line,
                Column = column
            };
        }

        public static VdfToken OpenBrace(int line, int column)
        {
            return new VdfToken
            {
                Kind = VdfTokenKind.OpenBrace,
                Line = line,
                Column = column
            };
        }

        public static VdfToken CloseBrace(int line, int column)
        {
            return new VdfToken
            {
                Kind = VdfTokenKind.CloseBrace,
                Line = line,
                Column = column
            };
        }
    }

    internal sealed class VdfTokenizer
    {
        private readonly string _content;
        private int _index;
        private int _line = 1;
        private int _column = 1;

        public VdfTokenizer(string content)
        {
            _content = content ?? string.Empty;
        }

        public VdfToken Next()
        {
            SkipWhitespace();
            if (_index >= _content.Length)
            {
                return VdfToken.End();
            }

            var line = _line;
            var column = _column;
            var c = _content[_index];

            if (c == '{')
            {
                Advance();
                return VdfToken.OpenBrace(line, column);
            }

            if (c == '}')
            {
                Advance();
                return VdfToken.CloseBrace(line, column);
            }

            if (c == '"')
            {
                return ReadQuotedString(line, column);
            }

            throw Error("Expected quoted string, '{' or '}'", line, column);
        }

        public VdfToken Peek()
        {
            var savedIndex = _index;
            var savedLine = _line;
            var savedColumn = _column;
            try
            {
                return Next();
            }
            finally
            {
                _index = savedIndex;
                _line = savedLine;
                _column = savedColumn;
            }
        }

        private VdfToken ReadQuotedString(int line, int column)
        {
            Advance(); // opening quote
            var raw = new StringBuilder();
            while (_index < _content.Length)
            {
                var c = _content[_index];
                if (c == '\\' && _index + 1 < _content.Length)
                {
                    raw.Append(c);
                    Advance();
                    raw.Append(_content[_index]);
                    Advance();
                    continue;
                }
                if (c == '"')
                {
                    Advance();
                    return VdfToken.String(VdfStringDecoder.Decode(raw.ToString()), line, column);
                }
                raw.Append(c);
                Advance();
            }
            throw Error("Unterminated quoted string", line, column);
        }

        private void SkipWhitespace()
        {
            while (_index < _content.Length)
            {
                var c = _content[_index];
                if (c == ' ' || c == '\t' || c == '\r')
                {
                    Advance();
                    continue;
                }
                if (c == '\n')
                {
                    _line++;
                    _column = 0;
                    Advance();
                    continue;
                }
                break;
            }
        }

        private void Advance()
        {
            if (_index < _content.Length)
            {
                _index++;
                _column++;
            }
        }

        private static VdfParseException Error(string message, int line, int column)
        {
            return new VdfParseException(message, line, column);
        }
    }
}
