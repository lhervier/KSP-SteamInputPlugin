using System.IO;
using System.Text;

namespace com.github.lhervier.ksp.Vdf
{
    /// <summary>
    /// Parser for Valve KeyValues text (VDF) files. Reads a single file; does not resolve #ref.
    /// All leaf values are decoded strings; a key repeated at the same level becomes a
    /// <see cref="VdfArray"/>.
    /// </summary>
    public static class VdfParser
    {
        public static VdfObject Parse(string content)
        {
            var tokenizer = new VdfTokenizer(content);
            var root = new VdfObject();
            ParseEntries(tokenizer, root, atRoot: true);

            var trailing = tokenizer.Peek();
            if (trailing.Kind != VdfTokenKind.End)
            {
                throw new VdfParseException("Unexpected content after end of document", trailing.Line, trailing.Column);
            }

            return root;
        }

        public static VdfObject ParseFile(string path)
        {
            var content = File.ReadAllText(path, Encoding.UTF8);
            return Parse(content);
        }

        private static void ParseEntries(VdfTokenizer tokenizer, VdfObject dict, bool atRoot = false)
        {
            while (true)
            {
                var peek = tokenizer.Peek();
                if (peek.Kind == VdfTokenKind.End || peek.Kind == VdfTokenKind.CloseBrace)
                {
                    if (atRoot && peek.Kind == VdfTokenKind.CloseBrace)
                    {
                        throw new VdfParseException("Unexpected '}'", peek.Line, peek.Column);
                    }
                    return;
                }

                var keyToken = tokenizer.Next();
                if (keyToken.Kind != VdfTokenKind.String)
                {
                    throw new VdfParseException("Expected key", keyToken.Line, keyToken.Column);
                }

                var valueToken = tokenizer.Next();
                object value;
                if (valueToken.Kind == VdfTokenKind.OpenBrace)
                {
                    var block = new VdfObject();
                    ParseEntries(tokenizer, block, atRoot: false);
                    var close = tokenizer.Next();
                    if (close.Kind != VdfTokenKind.CloseBrace)
                    {
                        throw new VdfParseException("Expected '}'", close.Line, close.Column);
                    }
                    value = block;
                }
                else if (valueToken.Kind == VdfTokenKind.String)
                {
                    value = valueToken.Value;
                }
                else
                {
                    throw new VdfParseException("Expected value or '{'", valueToken.Line, valueToken.Column);
                }

                dict.Add(keyToken.Value, value);
            }
        }
    }
}
