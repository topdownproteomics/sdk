using NUnit.Framework;
using System;
using System.Linq;
using TopDownProteomics.ProForma;

namespace TopDownProteomics.Tests
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

        /// <summary>
        /// Rule 6 with incompatible tag values. This is syntactically valid, but logically invalid. Pick up the error at the next level of validation.
        /// </summary>
        [Test]
        public void Rule6_WithModificationNames()
        {
            const string proFormaString = "[mass]+S[Methyl]EQVE[14]NCE";
            var term = _parser.ParseString(proFormaString);

            ProFormaTag tagMethyl = term.Tags[0];
            Assert.AreEqual(ProFormaKey.Mass, tagMethyl.Descriptors.Single().Key);
            Assert.AreEqual("Methyl", tagMethyl.Descriptors.Single().Value);

            ProFormaTag tag14 = term.Tags[1];
            Assert.AreEqual(ProFormaKey.Mass, tag14.Descriptors.Single().Key);
            Assert.AreEqual("14", tag14.Descriptors.Single().Value);
        }

        [Test]
        public void Rule7()
        {
            const string proFormaString = "[mass:-17.027]-SEQVENCE-[Amidation]";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("SEQVENCE", term.Sequence);
            Assert.IsNull(term.Tags);
            Assert.IsNotNull(term.NTerminalDescriptors);
            Assert.AreEqual(1, term.NTerminalDescriptors.Count);
            Assert.IsNotNull(term.CTerminalDescriptors);
            Assert.AreEqual(1, term.CTerminalDescriptors.Count);

            var nTerm = term.NTerminalDescriptors[0];
            Assert.AreEqual(ProFormaKey.Mass, nTerm.Key);
            Assert.AreEqual("-17.027", nTerm.Value);

            var cTerm = term.CTerminalDescriptors[0];
            Assert.AreEqual(ProFormaKey.Mod, cTerm.Key);
            Assert.AreEqual("Amidation", cTerm.Value);
        }

        [Test]
        public void Rule7andRule6()
        {
            const string proFormaString = "[mass]+[-17.027]-SEQ[14.05]VENCE";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("SEQVENCE", term.Sequence);
            Assert.IsNotNull(term.Tags);
            Assert.AreEqual(1, term.Tags.Count);
            Assert.IsNotNull(term.NTerminalDescriptors);
            Assert.AreEqual(1, term.NTerminalDescriptors.Count);
            Assert.IsNull(term.CTerminalDescriptors);

            var nTerm = term.NTerminalDescriptors[0];
            Assert.AreEqual(ProFormaKey.Mass, nTerm.Key);
            Assert.AreEqual("-17.027", nTerm.Value);

            ProFormaTag tag1 = term.Tags[0];
            Assert.AreEqual(2, tag1.Index);
            Assert.AreEqual(1, tag1.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Mass, tag1.Descriptors.Single().Key);
            Assert.AreEqual("14.05", tag1.Descriptors.Single().Value);
        }

        [Test]
        [TestCase("[mass]+S[mod:Methyl]EQVE[14]NCE")]
        [TestCase("[mass]+[mod:Methyl]-SEQVENCE")]
        [TestCase("[Methyl]-[mass]+SEQ[14.05]VENCE")]
        public void Rule6_7_Invalid(string proFormaString)
        {
            Assert.Throws<ProFormaParseException>(() => _parser.ParseString(proFormaString));
        }

        // Best Practice examples should be made into unit/integration tests
        //[Acetyl]-S[Phospho|mass:79.966331]GRGK[Acetyl|Unimod:1|mass:42.010565]QGGKARAKAKTRSSRAGLQFPVGRVHRLLRKGNYAERVGAGAPVYLAAVLEYLTAEILELAGNAARDNKKTRIIPRHLQLAIRNDEELNKLLGKVTIAQGGVLPNIQAVLLPKKT[Unimod:21]ESHHKAKGK
        //[Unimod]+[1]-S[21]GRGK[1]QGGKARAKAKTRSSRAGKVTIAQGGVLPNIQAVLLPKKT[21]ESHHKAKGK
        //MTLFQLREHWFVYKDDEKLTAFRNK[p-adenosine| N6-(phospho-5'-adenosine)-L-lysine (RESID)| RESID:AA0227| PSI-MOD:MOD:00232| N6AMPLys(PSI-MOD)]SMLFQRELRPNEEVTWK
        //MTLFQLDEKLTA[mass:-37.995001|info:unknown modification]FRNKSMLFQRELRPNEEVTWK

        //[Test]
        //public void BestPractice_ii()
        //{
        //    const string proFormaString = "[Unimod]+[1]-S[21]GRGK[1]QGGKARAKAKTRSSRAGKVTIAQGGVLPNIQAVLLPKKT[21]ESHHKAKGK";
        //    var term = _parser.ParseString(proFormaString);

        //    Assert.AreEqual("SGRGKQGGKARAKAKTRSSRAGKVTIAQGGVLPNIQAVLLPKKTESHHKAKGK", term.Sequence);
        //    Assert.IsNotNull(term.Tags);
        //    Assert.AreEqual(3, term.Tags.Count);
        //    Assert.IsNotNull(term.NTerminalDescriptors);
        //    Assert.AreEqual(1, term.NTerminalDescriptors.Count);
        //    Assert.IsNull(term.CTerminalDescriptors);

        //    var nTerm = term.NTerminalDescriptors[0];
        //    Assert.AreEqual(ProFormaKey.Mod, nTerm.Key);
        //    Assert.AreEqual("1", nTerm.Value);

        //    ProFormaTag tag1 = term.Tags[0];
        //    Assert.AreEqual(0, tag1.Index);
        //    Assert.AreEqual(1, tag1.Descriptors.Count);
        //    Assert.AreEqual(ProFormaKey.Mod, tag1.Descriptors.Single().Key);
        //    Assert.AreEqual("21", tag1.Descriptors.Single().Value);

        //    ProFormaTag tag5 = term.Tags[1];
        //    Assert.AreEqual(4, tag5.Index);
        //    Assert.AreEqual(1, tag5.Descriptors.Count);
        //    Assert.AreEqual(ProFormaKey.Mod, tag5.Descriptors.Single().Key);
        //    Assert.AreEqual("1", tag5.Descriptors.Single().Value);

        //    ProFormaTag tag44 = term.Tags[2];
        //    Assert.AreEqual(43, tag44.Index);
        //    Assert.AreEqual(1, tag44.Descriptors.Count);
        //    Assert.AreEqual(ProFormaKey.Mod, tag44.Descriptors.Single().Key);
        //    Assert.AreEqual("21", tag44.Descriptors.Single().Value);
        //}

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