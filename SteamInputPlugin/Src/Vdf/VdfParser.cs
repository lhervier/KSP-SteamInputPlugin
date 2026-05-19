using System.Collections.Generic;
using System.IO;
using System.Text;

namespace com.github.lhervier.ksp.Vdf
{
    /// <summary>
    /// Parser for Valve KeyValues text (VDF) files. Reads a single file; does not resolve #ref.
    /// All leaf values are decoded strings; duplicate keys at the same level become lists.
    /// </summary>
    public static class VdfParser
    {
        public static Dictionary<string, object> Parse(string content)
        {
            var tokenizer = new VdfTokenizer(content);
            var root = new Dictionary<string, object>();
            ParseEntries(tokenizer, root, atRoot: true);

            var trailing = tokenizer.Peek();
            if (trailing.Kind != VdfTokenKind.End)
            {
                throw new VdfParseException("Unexpected content after end of document", trailing.Line, trailing.Column);
            }

            return root;
        }

        public static Dictionary<string, object> ParseFile(string path)
        {
            var content = File.ReadAllText(path, Encoding.UTF8);
            return Parse(content);
        }

        private static void ParseEntries(VdfTokenizer tokenizer, Dictionary<string, object> dict, bool atRoot = false)
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
                    var block = new Dictionary<string, object>();
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

                AddEntry(dict, keyToken.Value, value);
            }
        }

        private static void AddEntry(Dictionary<string, object> dict, string key, object value)
        {
            object existing;
            if (!dict.TryGetValue(key, out existing))
            {
                dict[key] = value;
                return;
            }

            var list = existing as List<object>;
            if (list != null)
            {
                list.Add(value);
                return;
            }

            dict[key] = new List<object> { existing, value };
        }
    }
}
