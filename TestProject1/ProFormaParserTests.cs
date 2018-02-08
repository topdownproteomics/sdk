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
            Assert.AreEqual(ProFormaKey.Info, term.Tags.Single().Descriptors.Single().Key);
            Assert.AreEqual("test", term.Tags.Single().Descriptors.Single().Value);
        }

        [Test]
        public void MultipleDescriptorTag()
        {
            const string proFormaString = "SEQUEN[mod:Methyl|mass:+14.02]CE";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("SEQUENCE", term.Sequence);
            Assert.IsNotNull(term.Tags);
            Assert.AreEqual(1, term.Tags.Count);

            ProFormaTag tag = term.Tags.Single();
            Assert.AreEqual(5, tag.Index);
            Assert.AreEqual(2, tag.Descriptors.Count);

            Assert.AreEqual(ProFormaKey.Mod, tag.Descriptors.First().Key);
            Assert.AreEqual("Methyl", tag.Descriptors.First().Value);
            Assert.AreEqual(ProFormaKey.Mass, tag.Descriptors.Last().Key);
            Assert.AreEqual("+14.02", tag.Descriptors.Last().Value);
        }

        [Test]
        public void ValueOnlyDescriptor()
        {
            const string proFormaString = "PRO[Methyl]TEOFORM";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("PROTEOFORM", term.Sequence);
            Assert.IsNotNull(term.Tags);
            Assert.AreEqual(1, term.Tags.Count);
            Assert.AreEqual(2, term.Tags.Single().Index);
            Assert.AreEqual(1, term.Tags.Single().Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Mod, term.Tags.Single().Descriptors.Single().Key);
            Assert.AreEqual("Methyl", term.Tags.Single().Descriptors.Single().Value);
        }

        [Test]
        public void MixedDescriptor()
        {
            const string proFormaString = "SEQUEN[Methyl|mass:+14.02]CE";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("SEQUENCE", term.Sequence);
            Assert.IsNotNull(term.Tags);
            Assert.AreEqual(1, term.Tags.Count);

            ProFormaTag tag = term.Tags.Single();
            Assert.AreEqual(5, tag.Index);
            Assert.AreEqual(2, tag.Descriptors.Count);

            Assert.AreEqual(ProFormaKey.Mod, tag.Descriptors.First().Key);
            Assert.AreEqual("Methyl", tag.Descriptors.First().Value);
            Assert.AreEqual(ProFormaKey.Mass, tag.Descriptors.Last().Key);
            Assert.AreEqual("+14.02", tag.Descriptors.Last().Value);
        }

        [Test]
        public void EmptyDescriptor()
        {
            const string proFormaString = "PRO[]TEOFORM";
            Assert.Throws<ProFormaParseException>(() => _parser.ParseString(proFormaString));
        }

        [Test]
        public void MixedEmptyDescriptor()
        {
            const string proFormaString = "PRO[mod:Methyl|]TEOFORM";
            Assert.Throws<ProFormaParseException>(() => _parser.ParseString(proFormaString));
        }

        [Test]
        public void UnsupportedKey()
        {
            const string proFormaString = "PRO[xlink:Formaldehyde]TEOFORM";
            Assert.Throws<ProFormaParseException>(() => _parser.ParseString(proFormaString));
        }
    }
}