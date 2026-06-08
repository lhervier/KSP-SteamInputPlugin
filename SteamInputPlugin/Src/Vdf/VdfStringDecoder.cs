using System.Text;

namespace com.github.lhervier.ksp.steaminput.Vdf
{
    internal static class VdfStringDecoder
    {
        /// <summary>
        /// Decode Valve KeyValues escape sequences in a quoted string (without surrounding quotes).
        /// Mirrors MergeScripts/src/vdf-utils.js _decodeVdfString.
        /// </summary>
        public static string Decode(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s ?? string.Empty;
            }

            var result = new StringBuilder(s.Length);
            for (var i = 0; i < s.Length; i++)
            {
                if (s[i] == '\\' && i + 1 < s.Length)
                {
                    var next = s[++i];
                    switch (next)
                    {
                        case '\\': result.Append('\\'); break;
                        case '"': result.Append('"'); break;
                        case 'n': result.Append('\n'); break;
                        case 't': result.Append('\t'); break;
                        case 'r': result.Append('\r'); break;
                        default: result.Append(next); break;
                    }
                }
                else
                {
                    result.Append(s[i]);
                }
            }
            return result.ToString();
        }
    }
}
