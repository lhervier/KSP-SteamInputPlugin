using System.Collections.Generic;
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

        /// <summary>
        /// Shallow, best-effort reader. Scans a VDF file line by line for the given properties located
        /// exactly at <paramref name="path"/> (dotted, e.g. "controller_mappings" or
        /// "controller_mappings.group") and returns their values in the requested order — null for any
        /// property not found. Only the first block matching the path is considered, and the scan stops
        /// as soon as every property is found or that block is left, so it never reads more than needed.
        /// Assumes the regular VDF export layout (one "key" "value" per line, braces alone on a line);
        /// it is not a general-purpose VDF accessor.
        /// </summary>
        public static string[] ParseProperties(string file, string path, params string[] propertyNames)
        {
            using (TextReader reader = new StreamReader(file))
            {
                return ParseProperties(reader, path, propertyNames);
            }
        }

        /// <summary>
        /// <see cref="ParseProperties(string, string, string[])"/> reading from an arbitrary reader.
        /// </summary>
        public static string[] ParseProperties(TextReader reader, string path, params string[] propertyNames)
        {
            var result = new string[propertyNames.Length];

            // property name -> index in the result (first wins on duplicates)
            var wanted = new Dictionary<string, int>();
            for (var i = 0; i < propertyNames.Length; i++)
            {
                if (!wanted.ContainsKey(propertyNames[i]))
                {
                    wanted[propertyNames[i]] = i;
                }
            }

            var target = path.Split('.');
            var stack = new List<string>();
            string lastKey = null;
            var found = 0;

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var trimmed = line.Trim();
                if (trimmed.Length == 0)
                {
                    continue;
                }

                if (trimmed == "{")
                {
                    // A '{' always follows the key that names the block (regular layout).
                    stack.Add(lastKey);
                    lastKey = null;
                    continue;
                }

                if (trimmed == "}")
                {
                    // Leaving the target block: the first match is complete.
                    if (PathMatches(stack, target))
                    {
                        return result;
                    }
                    if (stack.Count > 0)
                    {
                        stack.RemoveAt(stack.Count - 1);
                    }
                    lastKey = null;
                    continue;
                }

                if (!TryReadKeyValue(trimmed, out string key, out string value))
                {
                    continue;
                }

                if (value == null)
                {
                    // Bare key: the next '{' opens its block.
                    lastKey = key;
                    continue;
                }

                // Scalar: collect only when sitting exactly at the requested path.
                if (PathMatches(stack, target) && wanted.TryGetValue(key, out int index) && result[index] == null)
                {
                    result[index] = value;
                    found++;
                    if (found == result.Length)
                    {
                        return result;
                    }
                }
            }

            return result;
        }

        private static bool PathMatches(List<string> stack, string[] target)
        {
            if (stack.Count != target.Length)
            {
                return false;
            }
            for (var i = 0; i < target.Length; i++)
            {
                if (stack[i] != target[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Reads the first quoted token (key) and, if present, the second quoted token (value, decoded)
        /// from a single line. Returns false if there is no quoted key.
        /// </summary>
        private static bool TryReadKeyValue(string line, out string key, out string value)
        {
            key = null;
            value = null;

            var k1 = line.IndexOf('"');
            if (k1 < 0)
            {
                return false;
            }
            var k2 = line.IndexOf('"', k1 + 1);
            if (k2 < 0)
            {
                return false;
            }
            key = line.Substring(k1 + 1, k2 - k1 - 1);

            var v1 = line.IndexOf('"', k2 + 1);
            if (v1 >= 0)
            {
                var v2 = line.IndexOf('"', v1 + 1);
                if (v2 >= 0)
                {
                    value = VdfStringDecoder.Decode(line.Substring(v1 + 1, v2 - v1 - 1));
                }
            }
            return true;
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
