using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SAIL.Tests
{
    [TestClass]
    public class SAILEncodingTests
    {
        [TestMethod]
        public void TestEncodeToJSON()
        {
            entities.SAILModData data = new entities.SAILModData();
            data.saveEntityMappings.Add(new entities.SaveEntityMapping(10, "fish1", "dizzy.pets"));
            data.saveEntityMappings.Add(new entities.SaveEntityMapping(21, "fish2", "dizzy.pets"));
            data.saveEntityMappings.Add(new entities.SaveEntityMapping(9, "dog", "dizzy.pets"));
            data.saveEntityMappings.Add(new entities.SaveEntityMapping(2, "cat", "dizzy.pets"));
            string json = utils.SAILModDataEncoder.EncodeSAILModData(data);

            Console.WriteLine(json);

        }

        [TestMethod]
        public void TestEscapeCharacters()
        {
            // Expected values are written as the literal text that should end up in the file.
            AssertEscapes("plain", "plain");
            AssertEscapes("", "");
            AssertEscapes("\n", "\\n");
            AssertEscapes("\t", "\\t");
            AssertEscapes("\r", "\\r");
            AssertEscapes("\\", "\\\\");
            AssertEscapes("\n\t\\", "\\n\\t\\\\");
            AssertEscapes("a\tb", "a\\tb");
            AssertEscapes("line1\r\nline2", "line1\\r\\nline2");

            // A backslash followed by 'n' is two characters of text, not a newline. It must come back as exactly that.
            AssertEscapes("\\n", "\\\\n");
        }

        [TestMethod]
        public void TestEscapedValueHasNoDelimiters()
        {
            string escaped = utils.SAILModDataEncoder.EscapeSpecialCharacters("a\tb\nc\rd");
            Assert.IsFalse(escaped.Contains("\t"));
            Assert.IsFalse(escaped.Contains("\n"));
            Assert.IsFalse(escaped.Contains("\r"));
        }

        [TestMethod]
        public void TestUnescapeCharacters()
        {
            Assert.AreEqual("\n", utils.SAILModDataEncoder.UnescapeSpecialCharacters("\\n"));
            Assert.AreEqual("\t", utils.SAILModDataEncoder.UnescapeSpecialCharacters("\\t"));
            Assert.AreEqual("\r", utils.SAILModDataEncoder.UnescapeSpecialCharacters("\\r"));
            Assert.AreEqual("\\", utils.SAILModDataEncoder.UnescapeSpecialCharacters("\\\\"));
            Assert.AreEqual("\\n", utils.SAILModDataEncoder.UnescapeSpecialCharacters("\\\\n"));
            Assert.AreEqual("", utils.SAILModDataEncoder.UnescapeSpecialCharacters(""));
        }

        [TestMethod]
        public void TestEscapeRoundTrip()
        {
            string[] samples =
            {
                "", "plain", "com.dizzy.petstore", "a\\b", "\\n", "\\\\", "trailing\\",
                "\t", "\n", "\r", "\r\n", "  spaces  ", "tab\there\nnewline\rcr\\slash",
                "|||"
            };
            foreach (string sample in samples)
                Assert.AreEqual(sample, RoundTrip(sample), "Round trip failed for: " + sample.Replace("\n", "<LF>").Replace("\t", "<TAB>").Replace("\r", "<CR>"));
        }

        [TestMethod]
        public void TestPipeCharacter()
        {
            Assert.AreEqual("\\|", utils.SAILModDataEncoder.EscapeSpecialCharacters("|"));
            Assert.AreEqual("|", utils.SAILModDataEncoder.UnescapeSpecialCharacters("\\|"));
        }

        /// <summary>
        /// Every string of up to four characters over an alphabet of the confusing characters has to survive a round trip.
        /// </summary>
        [TestMethod]
        public void TestEscapeRoundTripExhaustive()
        {
            char[] alphabet = { 'a', '\\', 't', 'n', 'r', '\t', '\n', '\r' };
            var current = new System.Collections.Generic.List<string> { "" };
            for (int length = 1; length <= 4; length++)
            {
                var next = new System.Collections.Generic.List<string>();
                foreach (string prefix in current)
                    foreach (char c in alphabet)
                        next.Add(prefix + c);

                foreach (string sample in next)
                    Assert.AreEqual(sample, RoundTrip(sample));
                current = next;
            }
        }

        // These two describe behavior that still needs to be added to UnescapeSpecialCharacters:
        // a dangling backslash is silently dropped, and a bad escape throws a bare Exception.
        [TestMethod]
        public void TestUnescapeRejectsDanglingBackslash()
        {
            Assert.ThrowsException<FormatException>(() => utils.SAILModDataEncoder.UnescapeSpecialCharacters("abc\\"));
        }

        [TestMethod]
        public void TestUnescapeRejectsUnknownEscape()
        {
            Assert.ThrowsException<FormatException>(() => utils.SAILModDataEncoder.UnescapeSpecialCharacters("\\q"));
        }

        private static void AssertEscapes(string raw, string expectedEscaped)
        {
            Assert.AreEqual(expectedEscaped, utils.SAILModDataEncoder.EscapeSpecialCharacters(raw));
        }

        private static string RoundTrip(string value)
        {
            return utils.SAILModDataEncoder.UnescapeSpecialCharacters(utils.SAILModDataEncoder.EscapeSpecialCharacters(value));
        }
    }
}
