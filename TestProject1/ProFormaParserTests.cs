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
        public void Rule6()
        {
            const string proFormaString = "[mass]+S[80]EQVE[14]NCE";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("SEQVENCE", term.Sequence);
            Assert.IsNotNull(term.Tags);
            Assert.AreEqual(2, term.Tags.Count);

            ProFormaTag tag80 = term.Tags[0];
            Assert.AreEqual(0, tag80.Index);
            Assert.AreEqual(1, tag80.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Mass, tag80.Descriptors.Single().Key);
            Assert.AreEqual("80", tag80.Descriptors.Single().Value);

            ProFormaTag tag14 = term.Tags[1];
            Assert.AreEqual(4, tag14.Index);
            Assert.AreEqual(1, tag14.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Mass, tag14.Descriptors.Single().Key);
            Assert.AreEqual("14", tag14.Descriptors.Single().Value);
        }

        [Test]
        public void Rule6_WithModificationNames()
        {
            const string proFormaString = "[mass]+S[Methyl]EQVE[14]NCE";
            var term = _parser.ParseString(proFormaString);

            ProFormaTag tagMethyl = term.Tags[0];
            Assert.AreEqual(ProFormaKey.Mod, tagMethyl.Descriptors.Single().Key);
            Assert.AreEqual("Methyl", tagMethyl.Descriptors.Single().Value);

            ProFormaTag tag14 = term.Tags[1];
            Assert.AreEqual(ProFormaKey.Mass, tag14.Descriptors.Single().Key);
            Assert.AreEqual("14", tag14.Descriptors.Single().Value);
        }

        [Test]
        [TestCase("PROTEOFXRM")]
        [TestCase("PROTEOF@RM")]
        [TestCase("proteoform")]
        [TestCase("    ")]
        [TestCase("----")]
        public void BadInput(string proFormaString)
        {
            Assert.Throws<ProFormaParseException>(() => _parser.ParseString(proFormaString));
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