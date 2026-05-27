using System.Linq;
using com.github.lhervier.ksp.Vdf;
using NUnit.Framework;

namespace com.github.lhervier.ksp.Tests
{
    /// <summary>
    /// Tests on VdfParser. Most assert on leaf strings and on VdfParseException
    /// (escape decoding, error handling with line/column, edge cases): these are
    /// representation-agnostic and survive the planned switch from
    /// Dictionary&lt;string, object&gt; to VdfObject/VdfArray.
    ///
    /// The "duplicate keys" section is the exception: it asserts on the current
    /// List&lt;object&gt; representation and will migrate to VdfObject/VdfArray unit
    /// tests after the refactor. It is kept here because collapsing repeated keys
    /// into a sequence is the parser's most refactor-sensitive behaviour.
    /// </summary>
    [TestFixture]
    public class VdfParserTests
    {
        private static string Leaf(VdfObject root, string key)
        {
            return root.GetString(key);
        }

        // ===============================================================================================
        // Escape decoding
        // ===============================================================================================

        [Test]
        public void DecodesKnownEscapeSequences()
        {
            // Verbatim string: backslashes are literal here, so "\n" below is really
            // backslash + n, i.e. a VDF escape (not a C# newline). "" is one quote.
            var root = VdfParser.Parse(@"
                ""newline""    ""a\nb""
                ""tab""        ""a\tb""
                ""return""     ""a\rb""
                ""backslash""  ""a\\b""
                ""quote""      ""a\""b""
            ");

            Assert.That(Leaf(root, "newline"), Is.EqualTo("a\nb"));
            Assert.That(Leaf(root, "tab"), Is.EqualTo("a\tb"));
            Assert.That(Leaf(root, "return"), Is.EqualTo("a\rb"));
            Assert.That(Leaf(root, "backslash"), Is.EqualTo("a\\b"));
            // An escaped quote does NOT terminate the string.
            Assert.That(Leaf(root, "quote"), Is.EqualTo("a\"b"));
        }

        [Test]
        public void UnknownEscape_KeepsCharLiteral()
        {
            // "\q" is an unknown escape: the backslash is dropped, the char kept as-is.
            var root = VdfParser.Parse(@"""key""   ""a\qb""");

            Assert.That(Leaf(root, "key"), Is.EqualTo("aqb"));
        }

        // ===============================================================================================
        // Edge cases
        // ===============================================================================================

        [Test]
        public void EmptyInput_ReturnsEmptyRoot()
        {
            Assert.That(VdfParser.Parse(""), Is.Empty);
        }

        [Test]
        public void WhitespaceOnlyInput_ReturnsEmptyRoot()
        {
            Assert.That(VdfParser.Parse("   \r\n\t  "), Is.Empty);
        }

        // ===============================================================================================
        // Error handling
        // ===============================================================================================

        [Test]
        public void Throws_OnUnexpectedCloseBraceAtRoot()
        {
            var ex = Assert.Throws<VdfParseException>(() => VdfParser.Parse("}"));

            Assert.That(ex.Message, Does.Contain("Unexpected '}'"));
            Assert.That(ex.Line, Is.EqualTo(1));
            Assert.That(ex.Column, Is.EqualTo(1));
        }

        [Test]
        public void Throws_WhenValueIsMissing()
        {
            // "k" is a key, but a '}' is found where a value or '{' is expected.
            var ex = Assert.Throws<VdfParseException>(() => VdfParser.Parse("\"k\" }"));

            Assert.That(ex.Message, Does.Contain("Expected value or '{'"));
            Assert.That(ex.Line, Is.EqualTo(1));
            Assert.That(ex.Column, Is.EqualTo(5));
        }

        [Test]
        public void Throws_WhenKeyIsNotAString()
        {
            // A '{' appears where a key is expected.
            var ex = Assert.Throws<VdfParseException>(() => VdfParser.Parse("{ }"));

            Assert.That(ex.Message, Does.Contain("Expected key"));
            Assert.That(ex.Line, Is.EqualTo(1));
            Assert.That(ex.Column, Is.EqualTo(1));
        }

        [Test]
        public void Throws_OnUnquotedToken()
        {
            var ex = Assert.Throws<VdfParseException>(() => VdfParser.Parse("key \"value\""));

            Assert.That(ex.Message, Does.Contain("Expected quoted string, '{' or '}'"));
            Assert.That(ex.Line, Is.EqualTo(1));
            Assert.That(ex.Column, Is.EqualTo(1));
        }

        [Test]
        public void Throws_OnUnterminatedString()
        {
            var ex = Assert.Throws<VdfParseException>(() => VdfParser.Parse("\"key\" \"val"));

            Assert.That(ex.Message, Does.Contain("Unterminated quoted string"));
            Assert.That(ex.Line, Is.EqualTo(1));
            Assert.That(ex.Column, Is.EqualTo(7));
        }

        [Test]
        public void Throws_OnUnbalancedBraces_MissingClose()
        {
            // Block is never closed: EOF reached while a '}' is expected.
            // EOF carries no position, so Line/Column are 0 and absent from the message.
            var ex = Assert.Throws<VdfParseException>(() => VdfParser.Parse("\"a\" { \"b\" \"c\""));

            Assert.That(ex.Message, Does.Contain("Expected '}'"));
            Assert.That(ex.Line, Is.EqualTo(0));
        }

        [Test]
        public void Throws_OnCloseBraceOnSecondLine_WithCorrectPosition()
        {
            // "a" "b" on line 1, then a stray '}' alone on line 2.
            var ex = Assert.Throws<VdfParseException>(() => VdfParser.Parse("\"a\" \"b\"\n}"));

            Assert.That(ex.Message, Does.Contain("Unexpected '}'"));
            Assert.That(ex.Line, Is.EqualTo(2));
            Assert.That(ex.Column, Is.EqualTo(1));
        }

        // ===============================================================================================
        // Duplicate keys -> VdfArray (representation-coupled: asserts on VdfObject/VdfArray)
        // ===============================================================================================

        [Test]
        public void SingleOccurrence_IsAStringNotAnArray()
        {
            var root = VdfParser.Parse(@"""k""   ""v""");

            // GetString returns it; GetArray normalizes it into a one-element array.
            Assert.That(root.GetString("k"), Is.EqualTo("v"));
            Assert.That(root.GetArray("k").Count, Is.EqualTo(1));
            Assert.That(root.GetArray("k").Strings().Single(), Is.EqualTo("v"));
        }

        [Test]
        public void RepeatedStringKey_BecomesArrayOfStrings_InOrder()
        {
            var root = VdfParser.Parse(@"
                ""k""   ""v1""
                ""k""   ""v2""
                ""k""   ""v3""
            ");

            VdfArray array = root.GetArray("k");
            Assert.That(array.IsObjectArray, Is.False);
            Assert.That(array.Strings(), Is.EqualTo(new[] { "v1", "v2", "v3" }));
        }

        [Test]
        public void RepeatedObjectKey_BecomesArrayOfObjects_InOrder()
        {
            var root = VdfParser.Parse(@"
                ""g""
                {
                    ""id""   ""1""
                }
                ""g""
                {
                    ""id""   ""2""
                }
            ");

            VdfArray array = root.GetArray("g");
            Assert.That(array.IsObjectArray, Is.True);

            var objects = array.Objects().ToList();
            Assert.That(objects, Has.Count.EqualTo(2));
            Assert.That(objects[0].GetString("id"), Is.EqualTo("1"));
            Assert.That(objects[1].GetString("id"), Is.EqualTo("2"));
        }

        [Test]
        public void RepeatedKey_MixingStringAndObject_IsRejected()
        {
            // Mixing a string and a block under the same repeated key is invalid VDF.
            var ex = Assert.Throws<VdfParseException>(() => VdfParser.Parse(@"
                ""x""   ""str""
                ""x""
                {
                    ""a""   ""b""
                }
            "));

            Assert.That(ex.Message, Does.Contain("mixes string and block values"));
        }

        [Test]
        public void RepeatedKey_MixingObjectThenString_IsRejected()
        {
            // Same rejection regardless of which kind appears first.
            var ex = Assert.Throws<VdfParseException>(() => VdfParser.Parse(@"
                ""x""
                {
                    ""a""   ""b""
                }
                ""x""   ""str""
            "));

            Assert.That(ex.Message, Does.Contain("mixes string and block values"));
        }
    }
}
