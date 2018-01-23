using NUnit.Framework;
using System;
using System.Linq;
using TestLibNamespace.ProForma;

namespace TestProject1
{
    [TestFixture]
    public class ProFormaParserTests
    {
        public static ProFormaParser _parser = new ProFormaParser();

        [Test]
        public void InvalidProFormaStrings()
        {
            Assert.Throws<ArgumentNullException>(() => _parser.ParseString(null));
            Assert.Throws<ArgumentNullException>(() => _parser.ParseString(string.Empty));
        }

        [Test]
        public void NoModifications()
        {
            const string proFormaString = "PROTEOFORM";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual(proFormaString, term.Sequence);
            Assert.IsNull(term.Tags);
        }

        [Test]
        public void SimpleTag()
        {
            const string proFormaString = "PRO[info:test]TEOFORM";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("PROTEOFORM", term.Sequence);
            Assert.IsNotNull(term.Tags);
            Assert.AreEqual(1, term.Tags.Count);
            Assert.AreEqual(2, term.Tags.Single().Index);
            Assert.AreEqual(1, term.Tags.Single().Descriptors.Count);
            Assert.AreEqual("info", term.Tags.Single().Descriptors.Single().Key);
            Assert.AreEqual("test", term.Tags.Single().Descriptors.Single().Value);
        }
    }
}