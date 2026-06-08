using System;

namespace com.github.lhervier.ksp.steaminput.Vdf
{
    public sealed class VdfParseException : Exception
    {
        public int Line { get; }
        public int Column { get; }

        public VdfParseException(string message, int line, int column)
            : base(FormatMessage(message, line, column))
        {
            Line = line;
            Column = column;
        }

        private static string FormatMessage(string message, int line, int column)
        {
            if (line > 0)
            {
                return string.Format("{0} (line {1}, column {2})", message, line, column);
            }
            return message;
        }
    }
}
