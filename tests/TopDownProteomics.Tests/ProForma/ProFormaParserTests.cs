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
        [TestCase("PRO[info:test]TEOFORM", "PROTEOFORM", "test")]
        [TestCase("PRO[info:test[nested]]TEOFORM", "PROTEOFORM", "test[nested]")]
        public void SimpleInfoTag(string proFormaString, string sequence, string value)
        {
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual(sequence, term.Sequence);
            Assert.IsNotNull(term.Tags);
            Assert.AreEqual(1, term.Tags.Count);
            Assert.AreEqual(2, term.Tags.Single().ZeroBasedStartIndex);
            Assert.AreEqual(1, term.Tags.Single().Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Info, term.Tags.Single().Descriptors.Single().Key);
            Assert.AreEqual(value, term.Tags.Single().Descriptors.Single().Value);
        }

        [Test]
        public void HandleExtraTagSpaces()
        {
            var term = _parser.ParseString("PRO[info:test]TEOFORM");
            Assert.AreEqual(ProFormaKey.Info, term.Tags.Single().Descriptors[0].Key);
            Assert.AreEqual("test", term.Tags.Single().Descriptors[0].Value);

            // Trim extra spaces from beginning of the descriptor
            term = _parser.ParseString("PRO[ info:test]TEOFORM");
            Assert.AreEqual(ProFormaKey.Info, term.Tags.Single().Descriptors[0].Key);
            Assert.AreEqual("test", term.Tags.Single().Descriptors[0].Value);

            term = _parser.ParseString("PRO[info:test ]TEOFORM");
            Assert.AreEqual(ProFormaKey.Info, term.Tags.Single().Descriptors[0].Key);
            Assert.AreEqual("test ", term.Tags.Single().Descriptors[0].Value);

            term = _parser.ParseString("PRO[     info:test  ]TEOFORM");
            Assert.AreEqual(ProFormaKey.Info, term.Tags.Single().Descriptors[0].Key);
            Assert.AreEqual("test  ", term.Tags.Single().Descriptors[0].Value);

            // Keep everything after the colon
            term = _parser.ParseString("PRO[info: test]TEOFORM");
            Assert.AreEqual(ProFormaKey.Info, term.Tags.Single().Descriptors[0].Key);
            Assert.AreEqual(" test", term.Tags.Single().Descriptors[0].Value);
        }

        [Test]
        public void MultipleDescriptorTag()
        {
            // Tweaking for v2

            const string proFormaString = "SEQUEN[Methyl|+14.02]CE";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("SEQUENCE", term.Sequence);
            Assert.IsNotNull(term.Tags);
            Assert.AreEqual(1, term.Tags.Count);

            ProFormaTag tag = term.Tags.Single();
            Assert.AreEqual(5, tag.ZeroBasedStartIndex);
            Assert.AreEqual(2, tag.Descriptors.Count);

            Assert.AreEqual(ProFormaKey.Name, tag.Descriptors[0].Key);
            Assert.AreEqual("Methyl", tag.Descriptors[0].Value);
            Assert.AreEqual(ProFormaKey.Mass, tag.Descriptors[1].Key);
            Assert.AreEqual("+14.02", tag.Descriptors[1].Value);
        }

        [Test]
        [TestCase("PRO[Methyl]TEOFORM", "PROTEOFORM", "Methyl")]
        [TestCase("PRO[Cation:Fe[III]]TEOFORM", "PROTEOFORM", "Cation:Fe[III]")]
        public void ValueOnlyDescriptor(string proFormaString, string sequence, string modName)
        {
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual(sequence, term.Sequence);
            Assert.IsNotNull(term.Tags);
            Assert.AreEqual(1, term.Tags.Count);
            Assert.AreEqual(2, term.Tags.Single().ZeroBasedStartIndex);
            Assert.AreEqual(1, term.Tags.Single().Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Name, term.Tags.Single().Descriptors.Single().Key);
            Assert.AreEqual(modName, term.Tags.Single().Descriptors.Single().Value);
        }

        [Test]
        public void MixedDescriptor()
        {
            // Tweaking for v2

            const string proFormaString = "SEQUEN[Methyl|+14.02]CE";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("SEQUENCE", term.Sequence);
            Assert.IsNotNull(term.Tags);
            Assert.AreEqual(1, term.Tags.Count);

            ProFormaTag tag = term.Tags.Single();
            Assert.AreEqual(5, tag.ZeroBasedStartIndex);
            Assert.AreEqual(2, tag.Descriptors.Count);

            Assert.AreEqual(ProFormaKey.Name, tag.Descriptors.First().Key);
            Assert.AreEqual("Methyl", tag.Descriptors.First().Value);
            Assert.AreEqual(ProFormaKey.Mass, tag.Descriptors.Last().Key);
            Assert.AreEqual("+14.02", tag.Descriptors.Last().Value);
        }

        [Test]
        [Ignore("Invalid in v2.")]
        public void Rule6()
        {
            const string proFormaString = "[mass]+S[80]EQVE[14]NCE";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("SEQVENCE", term.Sequence);
            Assert.IsNotNull(term.Tags);
            Assert.AreEqual(2, term.Tags.Count);

            ProFormaTag tag80 = term.Tags[0];
            Assert.AreEqual(0, tag80.ZeroBasedStartIndex);
            Assert.AreEqual(1, tag80.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Mass, tag80.Descriptors.Single().Key);
            Assert.AreEqual("80", tag80.Descriptors.Single().Value);

            ProFormaTag tag14 = term.Tags[1];
            Assert.AreEqual(4, tag14.ZeroBasedStartIndex);
            Assert.AreEqual(1, tag14.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Mass, tag14.Descriptors.Single().Key);
            Assert.AreEqual("14", tag14.Descriptors.Single().Value);
        }

        /// <summary>
        /// Rule 6 with incompatible tag values. This is syntactically valid, but logically invalid. Pick up the error at the next level of validation.
        /// </summary>
        [Test]
        [Ignore("Invalid in v2.")]
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
            // Tweaking for v2

            const string proFormaString = "[-17.027]-SEQVENCE-[Amidation]";
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
            Assert.AreEqual(ProFormaKey.Name, cTerm.Key);
            Assert.AreEqual("Amidation", cTerm.Value);
        }

        [Test]
        [Ignore("Invalid in v2.")]
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
            Assert.AreEqual(2, tag1.ZeroBasedStartIndex);
            Assert.AreEqual(1, tag1.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Mass, tag1.Descriptors.Single().Key);
            Assert.AreEqual("14.05", tag1.Descriptors.Single().Value);
        }

        [Test]
        [Ignore("Invalid in v2.")]
        [TestCase("[mass]+S[mod:Methyl]EQVE[14]NCE")]
        [TestCase("[mass]+[mod:Methyl]-SEQVENCE")]
        [TestCase("[Methyl]-[mass]+SEQ[14.05]VENCE")]
        public void Rule6_7_Invalid(string proFormaString)
        {
            Assert.Throws<ProFormaParseException>(() => _parser.ParseString(proFormaString));
        }

        [Test]
        [TestCase("[Acetyl]-[Phospho]?PROTEOFORM")] // terminal mods must be adjacent to sequence
        [TestCase("PROT[Phospho|#]EOFORMS[#]")] // empty group string
        public void AmbiguityRulesInvalid(string proFormaString)
        {
            Assert.Throws<ProFormaParseException>(() => _parser.ParseString(proFormaString));
        }

        [Test]
        public void BestPractice_i()
        {
            // Tweaking for v2

            const string proFormaString = "[Acetyl]-S[Phospho|+79.966331]GRGK[Acetyl|UNIMOD:1|+42.010565]QGGKARAKAKTRSSRAGLQFPVGRVHRLLRKGNYAERVGAGAPVYLAAVLEYLTAEILELAGNAARDNKKTRIIPRHLQLAIRNDEELNKLLGKVTIAQGGVLPNIQAVLLPKKT[UNIMOD:21]ESHHKAKGK";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("SGRGKQGGKARAKAKTRSSRAGLQFPVGRVHRLLRKGNYAERVGAGAPVYLAAVLEYLTAEILELAGNAARDNKKTRIIPRHLQLAIRNDEELNKLLGKVTIAQGGVLPNIQAVLLPKKTESHHKAKGK", term.Sequence);
            Assert.IsNotNull(term.Tags);
            Assert.AreEqual(3, term.Tags.Count);
            Assert.IsNotNull(term.NTerminalDescriptors);
            Assert.AreEqual(1, term.NTerminalDescriptors.Count);
            Assert.IsNull(term.CTerminalDescriptors);

            var nTerm = term.NTerminalDescriptors[0];
            Assert.AreEqual(ProFormaKey.Name, nTerm.Key);
            Assert.AreEqual("Acetyl", nTerm.Value);

            ProFormaTag tag1 = term.Tags[0];
            Assert.AreEqual(0, tag1.ZeroBasedStartIndex);
            Assert.AreEqual(2, tag1.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Name, tag1.Descriptors.First().Key);
            Assert.AreEqual("Phospho", tag1.Descriptors.First().Value);
            Assert.AreEqual(ProFormaKey.Mass, tag1.Descriptors.Last().Key);
            Assert.AreEqual("+79.966331", tag1.Descriptors.Last().Value);

            ProFormaTag tag5 = term.Tags[1];
            Assert.AreEqual(4, tag5.ZeroBasedStartIndex);
            Assert.AreEqual(3, tag5.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Name, tag5.Descriptors[0].Key);
            Assert.AreEqual("Acetyl", tag5.Descriptors[0].Value);
            Assert.AreEqual(ProFormaKey.Identifier, tag5.Descriptors[1].Key);
            Assert.AreEqual(ProFormaEvidenceType.Unimod, tag5.Descriptors[1].EvidenceType);
            Assert.AreEqual("UNIMOD:1", tag5.Descriptors[1].Value);
            Assert.AreEqual(ProFormaKey.Mass, tag5.Descriptors[2].Key);
            Assert.AreEqual("+42.010565", tag5.Descriptors[2].Value);

            ProFormaTag tag120 = term.Tags[2];
            Assert.AreEqual(119, tag120.ZeroBasedStartIndex);
            Assert.AreEqual(1, tag120.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Identifier, tag120.Descriptors.Single().Key);
            Assert.AreEqual(ProFormaEvidenceType.Unimod, tag120.Descriptors.Single().EvidenceType);
            Assert.AreEqual("UNIMOD:21", tag120.Descriptors.Single().Value);
        }

        [Test]
        [Ignore("Invalid in v2.")]
        public void BestPractice_ii()
        {
            const string proFormaString = "[Unimod]+[1]-S[21]GRGK[1]QGGKARAKAKTRSSRAGKVTIAQGGVLPNIQAVLLPKKT[21]ESHHKAKGK";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("SGRGKQGGKARAKAKTRSSRAGKVTIAQGGVLPNIQAVLLPKKTESHHKAKGK", term.Sequence);
            Assert.IsNotNull(term.Tags);
            Assert.AreEqual(3, term.Tags.Count);
            Assert.IsNotNull(term.NTerminalDescriptors);
            Assert.AreEqual(1, term.NTerminalDescriptors.Count);
            Assert.IsNull(term.CTerminalDescriptors);

            var nTerm = term.NTerminalDescriptors[0];
            Assert.AreEqual(ProFormaKey.Identifier, nTerm.Key);
            Assert.AreEqual(ProFormaEvidenceType.Unimod, nTerm.EvidenceType);
            Assert.AreEqual("1", nTerm.Value);

            ProFormaTag tag1 = term.Tags[0];
            Assert.AreEqual(0, tag1.ZeroBasedStartIndex);
            Assert.AreEqual(1, tag1.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Identifier, tag1.Descriptors.Single().Key);
            Assert.AreEqual(ProFormaEvidenceType.Unimod, tag1.Descriptors.Single().EvidenceType);
            Assert.AreEqual("21", tag1.Descriptors.Single().Value);

            ProFormaTag tag5 = term.Tags[1];
            Assert.AreEqual(4, tag5.ZeroBasedStartIndex);
            Assert.AreEqual(1, tag5.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Identifier, tag5.Descriptors.Single().Key);
            Assert.AreEqual(ProFormaEvidenceType.Unimod, tag5.Descriptors.Single().EvidenceType);
            Assert.AreEqual("1", tag5.Descriptors.Single().Value);

            ProFormaTag tag44 = term.Tags[2];
            Assert.AreEqual(43, tag44.ZeroBasedStartIndex);
            Assert.AreEqual(1, tag44.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Identifier, tag44.Descriptors.Single().Key);
            Assert.AreEqual(ProFormaEvidenceType.Unimod, tag44.Descriptors.Single().EvidenceType);
            Assert.AreEqual("21", tag44.Descriptors.Single().Value);
        }

        [Test]
        public void BestPractice_iii()
        {
            // Tweaking for v2

            const string proFormaString = "MTLFQLREHWFVYKDDEKLTAFRNK[p-adenosine|R:N6-(phospho-5'-adenosine)-L-lysine| RESID:AA0227| MOD:00232| N6AMPLys]SMLFQRELRPNEEVTWK";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("MTLFQLREHWFVYKDDEKLTAFRNKSMLFQRELRPNEEVTWK", term.Sequence);
            Assert.IsNotNull(term.Tags);
            Assert.AreEqual(1, term.Tags.Count);
            Assert.IsNull(term.NTerminalDescriptors);
            Assert.IsNull(term.CTerminalDescriptors);

            ProFormaTag tag25 = term.Tags[0];
            Assert.AreEqual(24, tag25.ZeroBasedStartIndex);
            Assert.AreEqual(5, tag25.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Name, tag25.Descriptors[0].Key);
            Assert.AreEqual("p-adenosine", tag25.Descriptors[0].Value);
            Assert.AreEqual(ProFormaKey.Name, tag25.Descriptors[1].Key);
            Assert.AreEqual(ProFormaEvidenceType.Resid, tag25.Descriptors[1].EvidenceType);
            Assert.AreEqual("N6-(phospho-5'-adenosine)-L-lysine", tag25.Descriptors[1].Value);
            Assert.AreEqual(ProFormaKey.Identifier, tag25.Descriptors[2].Key);
            Assert.AreEqual(ProFormaEvidenceType.Resid, tag25.Descriptors[2].EvidenceType);
            Assert.AreEqual("AA0227", tag25.Descriptors[2].Value);
            Assert.AreEqual(ProFormaKey.Identifier, tag25.Descriptors[3].Key);
            Assert.AreEqual(ProFormaEvidenceType.PsiMod, tag25.Descriptors[3].EvidenceType);
            Assert.AreEqual("MOD:00232", tag25.Descriptors[3].Value);
            Assert.AreEqual(ProFormaKey.Name, tag25.Descriptors[4].Key);
            Assert.AreEqual("N6AMPLys", tag25.Descriptors[4].Value);
        }

        [Test]
        public void BestPractice_iv()
        {
            // Tweaking for v2

            const string proFormaString = "MTLFQLDEKLTA[-37.995001|info:unknown modification]FRNKSMLFQRELRPNEEVTWK";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("MTLFQLDEKLTAFRNKSMLFQRELRPNEEVTWK", term.Sequence);
            Assert.IsNotNull(term.Tags);
            Assert.AreEqual(1, term.Tags.Count);
            Assert.IsNull(term.NTerminalDescriptors);
            Assert.IsNull(term.CTerminalDescriptors);

            ProFormaTag tag12 = term.Tags[0];
            Assert.AreEqual(11, tag12.ZeroBasedStartIndex);
            Assert.AreEqual(2, tag12.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Mass, tag12.Descriptors[0].Key); // Unimod
            Assert.AreEqual("-37.995001", tag12.Descriptors[0].Value);
            Assert.AreEqual(ProFormaKey.Info, tag12.Descriptors[1].Key); // RESID
            Assert.AreEqual("unknown modification", tag12.Descriptors[1].Value);
        }

        [Test]
        [TestCase("PRO[]TEOFORM")]
        [TestCase("PRO[mod:Methyl|]TEOFORM")]
        [TestCase("PRO[mod:jk :] lol]TEOFORM")]
        //[TestCase("PRO[fake:Formaldehyde]TEOFORM")]
        //[TestCase("PROTEOFXRM")]
        [TestCase("{Name}}PROTEOFORM")]
        [TestCase("{Name{}PROTEOFORM")]
        [TestCase("PROTEOF@RM")]
        [TestCase("proteoform")]
        [TestCase("    ")]
        [TestCase("----")]
        public void BadInput(string proFormaString)
        {
            Assert.Throws<ProFormaParseException>(() => _parser.ParseString(proFormaString));
        }

        #region Version 2.0 Tests
        [Test]
        [TestCase("EM[Oxidation]EVEES[Phospho]PEK")]
        [TestCase("EM[L-methionine sulfoxide]EVEES[O-phospho-L-serine]PEK")]
        [TestCase("EM[Oxidation]EVEES[Cation:Mg[II]]PEK")]
        public void ModificationNameUsage_4_2_1(string proFormaString)
        {
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("EMEVEESPEK", term.Sequence);
            Assert.AreEqual(2, term.Tags.Count);

            var desc1 = term.Tags[0].Descriptors.Single();
            var desc2 = term.Tags[1].Descriptors.Single();

            Assert.AreEqual(ProFormaKey.Name, desc1.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, desc1.EvidenceType);

            Assert.AreEqual(ProFormaKey.Name, desc2.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, desc2.EvidenceType);
        }

        [Test]
        public void ModificationNameUsage_4_2_1_Prefixes()
        {
            // RESID is R:
            var term = _parser.ParseString("EM[R:Methionine sulfone]EVEES[O-phospho-L-serine]PEK");
            var desc1 = term.Tags[0].Descriptors.Single();
            var desc2 = term.Tags[1].Descriptors.Single();

            Assert.AreEqual(ProFormaKey.Name, desc1.Key);
            Assert.AreEqual(ProFormaEvidenceType.Resid, desc1.EvidenceType);
            Assert.AreEqual("Methionine sulfone", desc1.Value);

            Assert.AreEqual(ProFormaKey.Name, desc2.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, desc2.EvidenceType);

            // XL-MOD is X:
            term = _parser.ParseString("EMEVTK[X:DSS#XL1]SESPEK");
            var tag1 = term.TagGroups.Single();

            Assert.AreEqual(ProFormaKey.Name, tag1.Key);
            Assert.AreEqual(ProFormaEvidenceType.XlMod, tag1.EvidenceType);
            Assert.AreEqual("DSS", tag1.Value);

            // GNO is G:
            term = _parser.ParseString("NEEYN[G:G59626AS]K");
            desc1 = term.Tags[0].Descriptors.Single();

            Assert.AreEqual(ProFormaKey.Name, desc1.Key);
            Assert.AreEqual(ProFormaEvidenceType.Gno, desc1.EvidenceType);
            Assert.AreEqual("G59626AS", desc1.Value);
        }

        // TODO: 4.2.1.1 -> Validation, not parsing

        [Test]
        [TestCase("EM[MOD:00719]EVEES[MOD:00046]PEK", ProFormaEvidenceType.PsiMod)]
        [TestCase("EM[UNIMOD:15]EVEES[UNIMOD:56]PEK", ProFormaEvidenceType.Unimod)]
        [TestCase("EM[RESID:AA0581]EVEES[RESID:AA0037]PEK", ProFormaEvidenceType.Resid)]
        public void ModificationAccessionNumbers_4_2_2(string proFormaString, ProFormaEvidenceType modType)
        {
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("EMEVEESPEK", term.Sequence);
            Assert.AreEqual(2, term.Tags.Count);

            var desc1 = term.Tags[0].Descriptors.Single();
            var desc2 = term.Tags[1].Descriptors.Single();

            Assert.AreEqual(modType, desc1.EvidenceType);
            Assert.AreEqual(modType, desc2.EvidenceType);
        }

        [Test]
        public void Crosslinkers_XL_MOD_4_2_3()
        {
            // Single group
            var term = _parser.ParseString("EMEVTK[XLMOD:02001#XL1]SESPEK[#XL1]");
            Assert.IsNull(term.Tags);
            Assert.AreEqual(1, term.TagGroups?.Count);

            var tagGroup = term.TagGroups.Single();

            Assert.AreEqual("XL1", tagGroup.Name);
            Assert.AreEqual(ProFormaKey.Identifier, tagGroup.Key);
            Assert.AreEqual(ProFormaEvidenceType.XlMod, tagGroup.EvidenceType);
            Assert.AreEqual("XLMOD:02001", tagGroup.Value);
            Assert.AreEqual(2, tagGroup.Members.Count);
            Assert.AreEqual(5, tagGroup.Members[0].ZeroBasedStartIndex);
            Assert.AreEqual(11, tagGroup.Members[1].ZeroBasedStartIndex);

            // Multiple groups
            term = _parser.ParseString("EMK[XLMOD:02000#XL1]EVTK[XLMOD:02001#XL2]SESK[#XL1]PEK[#XL2]");
            Assert.IsNull(term.Tags);
            Assert.AreEqual(2, term.TagGroups?.Count);

            var tagGroup1 = term.TagGroups.Single(x => x.Name == "XL1");
            var tagGroup2 = term.TagGroups.Single(x => x.Name == "XL2");

            Assert.AreEqual("XL1", tagGroup1.Name);
            Assert.AreEqual(ProFormaKey.Identifier, tagGroup1.Key);
            Assert.AreEqual(ProFormaEvidenceType.XlMod, tagGroup1.EvidenceType);
            Assert.AreEqual("XLMOD:02000", tagGroup1.Value);
            Assert.AreEqual(2, tagGroup1.Members.Count);
            Assert.AreEqual(2, tagGroup1.Members[0].ZeroBasedStartIndex);
            Assert.AreEqual(10, tagGroup1.Members[1].ZeroBasedStartIndex);

            Assert.AreEqual("XL2", tagGroup2.Name);
            Assert.AreEqual(ProFormaKey.Identifier, tagGroup2.Key);
            Assert.AreEqual(ProFormaEvidenceType.XlMod, tagGroup2.EvidenceType);
            Assert.AreEqual("XLMOD:02001", tagGroup2.Value);
            Assert.AreEqual(2, tagGroup2.Members.Count);
            Assert.AreEqual(6, tagGroup2.Members[0].ZeroBasedStartIndex);
            Assert.AreEqual(13, tagGroup2.Members[1].ZeroBasedStartIndex);

            // "Dead end" crosslinks
            term = _parser.ParseString("EMEVTK[XLMOD:02001#XL1]SESPEK");
            Assert.IsNull(term.Tags);
            Assert.AreEqual(1, term.TagGroups?.Count);

            tagGroup = term.TagGroups.Single();

            Assert.AreEqual("XL1", tagGroup.Name);
            Assert.AreEqual(ProFormaKey.Identifier, tagGroup.Key);
            Assert.AreEqual(ProFormaEvidenceType.XlMod, tagGroup.EvidenceType);
            Assert.AreEqual("XLMOD:02001", tagGroup.Value);
            Assert.AreEqual(1, tagGroup.Members.Count);
            Assert.AreEqual(5, tagGroup.Members[0].ZeroBasedStartIndex);
        }

        [Test]
        [TestCase("SEK[XLMOD:02001#XL1]UENCE\\EMEVTK[XLMOD:02001#XL1]SESPEK")]
        [TestCase("SEK[XLMOD:02001#XL1]UENCE\\EMEVTK[#XL1]SESPEK")]
        public void Crosslinks_4_2_3_No_Interchain(string proFormaString)
        {
            // Always throw, no support for inter-chain crosslinks
            Assert.Throws<ProFormaParseException>(() => _parser.ParseString(proFormaString));
        }

        [Test]
        [TestCase("EVTSEKC[MOD:00034#XL1]LEMSC[#XL1]EFD", ProFormaKey.Identifier, ProFormaEvidenceType.PsiMod, "MOD:00034")]
        [TestCase("EVTSEKC[L-cystine (cross-link)#XL1]LEMSC[#XL1]EFD", ProFormaKey.Name, ProFormaEvidenceType.None, "L-cystine (cross-link)")]
        [TestCase("EVTSEKC[X:Disulfide#XL1]LEMSC[#XL1]EFD", ProFormaKey.Name, ProFormaEvidenceType.XlMod, "Disulfide")]
        public void Crosslinks_4_2_3_Disulfides(string proFormaString, ProFormaKey proFormaKey, ProFormaEvidenceType evidenceType, string value)
        {
            var term = _parser.ParseString(proFormaString);
            Assert.IsNull(term.Tags);
            Assert.AreEqual(1, term.TagGroups?.Count);

            var tagGroup = term.TagGroups.Single();

            Assert.AreEqual("XL1", tagGroup.Name);
            Assert.AreEqual(proFormaKey, tagGroup.Key);
            Assert.AreEqual(evidenceType, tagGroup.EvidenceType);
            Assert.AreEqual(value, tagGroup.Value);
            Assert.AreEqual(2, tagGroup.Members.Count);
            Assert.AreEqual(6, tagGroup.Members[0].ZeroBasedStartIndex);
            Assert.AreEqual(11, tagGroup.Members[1].ZeroBasedStartIndex);
        }

        [Test]
        public void Crosslinks_4_2_3_Extra_Descriptors()
        {
            // If a tag with a group contains another descriptor, it is considered to be NOT part of that group.

            var term = _parser.ParseString("EMEVTK[XLMOD:02001#XL1|info:stuff]SESPEK[#XL1]");
            Assert.AreEqual(1, term.Tags?.Count);
            Assert.AreEqual(1, term.TagGroups?.Count);
        }

        [Test]
        public void Glycans_GNO_MOD_4_2_4()
        {
            var term = _parser.ParseString("YPVLN[GNO:G62765YT]VTMPN[GNO:G02815KT]NSNGKFDK");
            var desc1 = term.Tags[0].Descriptors.Single();
            var desc2 = term.Tags[1].Descriptors.Single();

            Assert.AreEqual(ProFormaKey.Identifier, desc1.Key);
            Assert.AreEqual(ProFormaEvidenceType.Gno, desc1.EvidenceType);
            Assert.AreEqual("GNO:G62765YT", desc1.Value);

            Assert.AreEqual(ProFormaKey.Identifier, desc2.Key);
            Assert.AreEqual(ProFormaEvidenceType.Gno, desc2.EvidenceType);
            Assert.AreEqual("GNO:G02815KT", desc2.Value);
        }

        [Test]
        public void DeltaMassNotation_4_2_5()
        {
            // Add evidence type to descriptor to handle prefixes.

            // No prefixes
            var term = _parser.ParseString("EM[+15.9949]EVEES[-79.9663]PEK");
            var desc1 = term.Tags[0].Descriptors.Single();
            var desc2 = term.Tags[1].Descriptors.Single();

            Assert.AreEqual(ProFormaKey.Mass, desc1.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, desc1.EvidenceType);
            Assert.AreEqual("+15.9949", desc1.Value);

            Assert.AreEqual(ProFormaKey.Mass, desc2.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, desc2.EvidenceType);
            Assert.AreEqual("-79.9663", desc2.Value);

            // Prefixes
            // TODO: One of these should not validate because these are theoretical masses and must match ontology exactly.
            term = _parser.ParseString("EM[U:+15.9949]EVEES[M:+79.9663]PEK");
            desc1 = term.Tags[0].Descriptors.Single();
            desc2 = term.Tags[1].Descriptors.Single();

            Assert.AreEqual(ProFormaKey.Mass, desc1.Key);
            Assert.AreEqual(ProFormaEvidenceType.Unimod, desc1.EvidenceType);
            Assert.AreEqual("+15.9949", desc1.Value);

            Assert.AreEqual(ProFormaKey.Mass, desc2.Key);
            Assert.AreEqual(ProFormaEvidenceType.PsiMod, desc2.EvidenceType);
            Assert.AreEqual("+79.9663", desc2.Value);

            // Observed mass
            term = _parser.ParseString("EM[U:+15.995]EVEES[Obs:+79.978]PEK");
            desc1 = term.Tags[0].Descriptors.Single();
            desc2 = term.Tags[1].Descriptors.Single();

            Assert.AreEqual(ProFormaKey.Mass, desc1.Key);
            Assert.AreEqual(ProFormaEvidenceType.Unimod, desc1.EvidenceType);
            Assert.AreEqual("+15.995", desc1.Value);

            Assert.AreEqual(ProFormaKey.Mass, desc2.Key);
            Assert.AreEqual(ProFormaEvidenceType.Observed, desc2.EvidenceType);
            Assert.AreEqual("+79.978", desc2.Value);
        }

        [Test]
        public void GapOfKnownMass_4_2_6()
        {
            // Parse straight, consider some validation change (e.g. force a mass to be specified, etc.)
            var term = _parser.ParseString("RTAAX[+367.0537]WT");

            Assert.AreEqual(1, term.Tags.Count);
            var desc1 = term.Tags.Single().Descriptors.Single();

            Assert.AreEqual(ProFormaKey.Mass, desc1.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, desc1.EvidenceType);
            Assert.AreEqual("+367.0537", desc1.Value);

        }

        [Test]
        public void ChemicalFormulas_4_2_7()
        {
            var term = _parser.ParseString("SEQUEN[Formula:C12H20O2]CE");
            var desc1 = term.Tags.Single().Descriptors.Single();

            Assert.AreEqual(ProFormaKey.Formula, desc1.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, desc1.EvidenceType);
            Assert.AreEqual("C12H20O2", desc1.Value);

            // TODO: Make sure all of these cases below are handled in the formula parser and validation

            // SEQUEN[Formula:C12 H20 O2]CE
            // SEQUEN[Formula:HN-1O2]CE
            // SEQUEN[Formula:[13C2][12C-2]H2N]CE
            // SEQUEN[Formula:[13C2]C-2H2N]CE
        }

        [Test]
        public void GlycanComposition_4_2_8()
        {
            var term = _parser.ParseString("SEQUEN[Glycan:HexNAc1Hex2]CE");
            var desc1 = term.Tags.Single().Descriptors.Single();

            Assert.AreEqual(ProFormaKey.Glycan, desc1.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, desc1.EvidenceType);
            Assert.AreEqual("HexNAc1Hex2", desc1.Value);
        }

        [Test]
        public void TerminalModifications_4_3_1()
        {
            var term = _parser.ParseString("[iTRAQ4plex]-EM[Hydroxylation]EVNES[Phospho]PEK");
            Assert.AreEqual(2, term.Tags.Count);
            Assert.IsNotNull(term.NTerminalDescriptors);
            Assert.IsNull(term.CTerminalDescriptors);

            var desc1 = term.NTerminalDescriptors.Single();
            Assert.AreEqual(ProFormaKey.Name, desc1.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, desc1.EvidenceType);
            Assert.AreEqual("iTRAQ4plex", desc1.Value);

            // N and C term
            term = _parser.ParseString("[iTRAQ4plex]-EM[U:Hydroxylation]EVNES[Phospho]PEK[iTRAQ4plex]-[Methyl]");
            Assert.AreEqual(3, term.Tags.Count);
            Assert.IsNotNull(term.NTerminalDescriptors);
            Assert.IsNotNull(term.CTerminalDescriptors);

            desc1 = term.NTerminalDescriptors.Single();
            Assert.AreEqual(ProFormaKey.Name, desc1.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, desc1.EvidenceType);
            Assert.AreEqual("iTRAQ4plex", desc1.Value);

            desc1 = term.CTerminalDescriptors.Single();
            Assert.AreEqual(ProFormaKey.Name, desc1.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, desc1.EvidenceType);
            Assert.AreEqual("Methyl", desc1.Value);

            // Check for using negative delta mass on C terminus... might interfere with the dash notation
            term = _parser.ParseString("EMEVNESPEK-[-15.9949]");
            Assert.IsNull(term.Tags);
            Assert.IsNull(term.NTerminalDescriptors);
            Assert.IsNotNull(term.CTerminalDescriptors);

            desc1 = term.CTerminalDescriptors.Single();
            Assert.AreEqual(ProFormaKey.Mass, desc1.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, desc1.EvidenceType);
            Assert.AreEqual("-15.9949", desc1.Value);
        }

        [Test]
        public void LabileModifications_4_3_2()
        {
            // Add labile descriptor list to term
            var term = _parser.ParseString("{Glycan:Hex}EM[U:Hydroxylation]EVNES[Phospho]PEK[iTRAQ4plex]");
            Assert.AreEqual(3, term.Tags.Count);

            var desc1 = term.LabileDescriptors.Single();

            Assert.AreEqual(ProFormaKey.Glycan, desc1.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, desc1.EvidenceType);
            Assert.AreEqual("Hex", desc1.Value);

            Assert.AreEqual("Hydroxylation", term.Tags.First().Descriptors.Single().Value); // Make sure next tag is ok

            // Labile and terminal mods
            term = _parser.ParseString("{Glycan:Hex}[iTRAQ4plex]-EM[Hydroxylation]EVNES[Phospho]PEK[iTRAQ4plex]");
            Assert.AreEqual(3, term.Tags.Count);
            Assert.IsNotNull(term.LabileDescriptors);
            Assert.IsNotNull(term.NTerminalDescriptors);
            Assert.IsNull(term.CTerminalDescriptors);

            desc1 = term.LabileDescriptors.Single();

            Assert.AreEqual(ProFormaKey.Glycan, desc1.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, desc1.EvidenceType);
            Assert.AreEqual("Hex", desc1.Value);

            desc1 = term.NTerminalDescriptors.Single();

            Assert.AreEqual(ProFormaKey.Name, desc1.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, desc1.EvidenceType);
            Assert.AreEqual("iTRAQ4plex", desc1.Value);

            Assert.AreEqual("Hydroxylation", term.Tags.First().Descriptors.Single().Value); // Make sure next tag is ok
        }

        [Test]
        public void Ambiguity_UnknownPosition_4_4_1()
        {
            // Use unlocalized list on term.
            var term = _parser.ParseString("[Phospho]?EM[Hydroxylation]EVTSESPEK");
            Assert.AreEqual(1, term.Tags?.Count);
            Assert.AreEqual(1, term.UnlocalizedTags?.Count);

            var tag = term.Tags.Single().Descriptors.Single();
            Assert.AreEqual("Hydroxylation", tag.Value);
            Assert.AreEqual(ProFormaKey.Name, tag.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, tag.EvidenceType);

            var unlocal = term.UnlocalizedTags.Single().Descriptors.Single();
            Assert.AreEqual(1, term.UnlocalizedTags.Single().Count);
            Assert.AreEqual("Phospho", unlocal.Value);
            Assert.AreEqual(ProFormaKey.Name, unlocal.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, unlocal.EvidenceType);


            // Check multiple unlocalized mods with a terminal mod
            term = _parser.ParseString("[Phospho][Phospho2]?[Acetyl]-EM[Hydroxylation]EVTSESPEK");
            Assert.AreEqual(1, term.Tags?.Count);
            Assert.IsNull(term.CTerminalDescriptors);
            Assert.AreEqual(1, term.NTerminalDescriptors?.Count);
            Assert.AreEqual(2, term.UnlocalizedTags?.Count);

            var nTerm = term.NTerminalDescriptors.Single();
            Assert.AreEqual("Acetyl", nTerm.Value);

            unlocal = term.UnlocalizedTags.First().Descriptors.Single();
            Assert.AreEqual(1, term.UnlocalizedTags.First().Count);
            Assert.AreEqual("Phospho", unlocal.Value);
            var unlocal2 = term.UnlocalizedTags.Last().Descriptors.Single();
            Assert.AreEqual(1, term.UnlocalizedTags.Last().Count);
            Assert.AreEqual("Phospho2", unlocal2.Value);


            // Check ^{count} format
            term = _parser.ParseString("[Phospho]^2[Methyl]?[Acetyl]-EM[Hydroxylation]EVTSESPEK");
            Assert.AreEqual(1, term.Tags?.Count);
            Assert.IsNull(term.CTerminalDescriptors);
            Assert.AreEqual(1, term.NTerminalDescriptors?.Count);
            Assert.AreEqual(2, term.UnlocalizedTags?.Count);

            nTerm = term.NTerminalDescriptors.Single();
            Assert.AreEqual("Acetyl", nTerm.Value);

            unlocal = term.UnlocalizedTags.First().Descriptors.Single();
            Assert.AreEqual(2, term.UnlocalizedTags.First().Count);
            Assert.AreEqual("Phospho", unlocal.Value);
            unlocal2 = term.UnlocalizedTags.Last().Descriptors.Single();
            Assert.AreEqual(1, term.UnlocalizedTags.Last().Count);
            Assert.AreEqual("Methyl", unlocal2.Value);


            // INVALID to have terminal mod before unlocalized mods
            Assert.Throws<ProFormaParseException>(() => _parser.ParseString("[Acetyl]-[Phospho]^2?EM[Hydroxylation]EVTSESPEK"));
        }

        [Test]
        public void Ambiguity_PossiblePositions_4_4_2()
        {
            // This is read as a named group 'g1' indicates that a phosphorylation exists on either T5, S6 or S8
            var term = _parser.ParseString("EM[Oxidation]EVT[#g1]S[#g1]ES[Phospho#g1]PEK");
            Assert.AreEqual(1, term.Tags?.Count);
            Assert.IsNull(term.UnlocalizedTags);
            Assert.AreEqual(1, term.TagGroups.Count);

            var group = term.TagGroups.Single();
            Assert.AreEqual("g1", group.Name);
            Assert.AreEqual("Phospho", group.Value);
            Assert.AreEqual(ProFormaKey.Name, group.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, group.EvidenceType);
            Assert.AreEqual(3, group.Members?.Count);
            CollectionAssert.AreEquivalent(new[] { 4, 5, 7 }, group.Members.Select(x => x.ZeroBasedStartIndex));

            // The following example is not valid because a single preferred location must be chosen for a modification:
            Assert.Throws<ProFormaParseException>(() => _parser.ParseString("EM[Oxidation]EVT[#g1]S[Phospho#g1]ES[Phospho#g1]PEK"));
        }

        [Test]
        public void Ambiguity_Ranges_4_4_3()
        {
            // Ranges of amino acids as possible locations for the modifications may be represented using parentheses within the amino acid sequence.
            var term = _parser.ParseString("PROT(EOSFORMS)[+19.0523]ISK");
            Assert.AreEqual(1, term.Tags?.Count);
            var tag = term.Tags.Single();
            Assert.AreEqual(4, tag.ZeroBasedStartIndex);
            Assert.AreEqual(11, tag.ZeroBasedEndIndex);

            // Range must end next to a tag
            Assert.Throws<ProFormaParseException>(() => _parser.ParseString("PROT(EOSFO)RMS[+19.0523]ISK"));

            // Range with a tag inside
            term = _parser.ParseString("PROT(EOC[Carbamidomethyl]FORMS)[+19.0523]ISK");
            Assert.AreEqual(2, term.Tags?.Count);
            tag = term.Tags.First();
            Assert.AreEqual(6, tag.ZeroBasedStartIndex);
            Assert.AreEqual(6, tag.ZeroBasedEndIndex);
            Assert.AreEqual("Carbamidomethyl", tag.Descriptors.Single().Value);

            var tag2 = term.Tags.Last();
            Assert.AreEqual(4, tag2.ZeroBasedStartIndex);
            Assert.AreEqual(11, tag2.ZeroBasedEndIndex);
            Assert.AreEqual("+19.0523", tag2.Descriptors.Single().Value);

            // Overlapping ranges represent a more complex case and are not yet supported, and so, the following example would NOT be valid:
            Assert.Throws<ProFormaParseException>(() => _parser.ParseString("P(ROT(EOSFORMS)[+19.0523]IS)[+19.0523]K"));

            // Ranges + groups + scores
            term = _parser.ParseString("PROT(EOSFORMS)[+19.0523#g1(0.01)]ISK[#g1(0.99)]");
            Assert.IsNull(term.Tags);
            Assert.AreEqual(1, term.TagGroups?.Count);
            var member1 = term.TagGroups.Single().Members.First();
            Assert.AreEqual(4, member1.ZeroBasedStartIndex);
            Assert.AreEqual(11, member1.ZeroBasedEndIndex);
            Assert.AreEqual(0.01, member1.Weight, 0.001);
            var member2 = term.TagGroups.Single().Members.Last();
            Assert.AreEqual(14, member2.ZeroBasedStartIndex);
            Assert.AreEqual(14, member2.ZeroBasedEndIndex);
            Assert.AreEqual(0.99, member2.Weight, 0.001);

            // More complex with inner tag not part of the group
            term = _parser.ParseString("PR[#g1(0.91)]OT(EOC[Carbamidomethyl]FORMS)[+19.05233#g1(0.09)]ISK");
            Assert.AreEqual(1, term.Tags?.Count);
            Assert.AreEqual(1, term.TagGroups?.Count);
            member1 = term.TagGroups.Single().Members.First();
            Assert.AreEqual(1, member1.ZeroBasedStartIndex);
            Assert.AreEqual(1, member1.ZeroBasedEndIndex);
            Assert.AreEqual(0.91, member1.Weight, 0.001);
            member2 = term.TagGroups.Single().Members.Last();
            Assert.AreEqual(4, member2.ZeroBasedStartIndex);
            Assert.AreEqual(11, member2.ZeroBasedEndIndex);
            Assert.AreEqual(0.09, member2.Weight, 0.001);
        }

        [Test]
        public void Ambiguity_PossiblePositionsWithScores_4_4_4()
        {
            // The values of the modification localization scores can be indicated in parentheses within the same group and brackets. 
            var term = _parser.ParseString("EM[Oxidation]EVT[#g1(0.01)]S[#g1(0.09)]ES[Phospho#g1(0.90)]PEK");
            Assert.AreEqual(1, term.Tags?.Count);
            Assert.AreEqual(1, term.TagGroups?.Count);
            var group = term.TagGroups.Single();
            Assert.AreEqual("g1", group.Name);
            Assert.AreEqual("Phospho", group.Value);
            Assert.AreEqual(3, group.Members.Count);

            var member0 = group.Members[0];
            Assert.AreEqual(4, member0.ZeroBasedStartIndex);
            Assert.AreEqual(0.01, member0.Weight, 0.001);
            var member1 = group.Members[1];
            Assert.AreEqual(5, member1.ZeroBasedStartIndex);
            Assert.AreEqual(0.09, member1.Weight, 0.001);
            var member2 = group.Members[2];
            Assert.AreEqual(7, member2.ZeroBasedStartIndex);
            Assert.AreEqual(0.90, member2.Weight, 0.001);

            // Weights must be double values
            Assert.Throws<ProFormaParseException>(() => _parser.ParseString("EM[Oxidation]EVT[#g1(0.01)]S[#g1(ABC)]ES[Phospho#g1(0.90)]PEK"));

            // The additional option to represent localisation scores is to leave the position of the modification as unknown using the ‘?’ notation, 
            //  but report the localization modification scores at specific sites.
            term = _parser.ParseString("[Phospho#s1]?EM[Oxidation]EVT[#s1(0.01)]S[#s1(0.09)]ES[#s1(0.90)]PEK");
            Assert.AreEqual(1, term.Tags?.Count);
            Assert.AreEqual(1, term.TagGroups?.Count);
            Assert.IsNull(term.UnlocalizedTags);
            group = term.TagGroups.Single();
            Assert.AreEqual("s1", group.Name);
            Assert.AreEqual("Phospho", group.Value);
            Assert.AreEqual(3, group.Members.Count);

            member0 = group.Members[0];
            Assert.AreEqual(4, member0.ZeroBasedStartIndex);
            Assert.AreEqual(0.01, member0.Weight, 0.001);
            member1 = group.Members[1];
            Assert.AreEqual(5, member1.ZeroBasedStartIndex);
            Assert.AreEqual(0.09, member1.Weight, 0.001);
            member2 = group.Members[2];
            Assert.AreEqual(7, member2.ZeroBasedStartIndex);
            Assert.AreEqual(0.90, member2.Weight, 0.001);
        }

        [Test]
        public void NoMultipleModificationsSameSite_4_5()
        {
            // Currently, there is no need to chain two mods together on the same residue, since complex glycans are not explicitly supported (see Section 3.4).
            //  The solution in those rare cases not involving glycans is to have a single PSI-MOD/RESID  entry for the combination of mods.
            Assert.Throws<ProFormaParseException>(() => _parser.ParseString("EM[Oxidation][Phospho]EVTSESPEK"));
        }

        [Test]
        public void GlobalModifications_4_6()
        {
            // Use Fixed Modification LIST on term

            // Representation of isotopes
            var term = _parser.ParseString("<13C>ATPEILTVNSIGQLK");
            Assert.IsNull(term.Tags);
            Assert.AreEqual(1, term.GlobalModifications.Count);

            var globalMod = term.GlobalModifications.Single().Descriptors.Single();
            Assert.AreEqual(ProFormaKey.Name, globalMod.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, globalMod.EvidenceType);
            Assert.AreEqual("13C", globalMod.Value);
            Assert.IsNull(term.GlobalModifications.Single().TargetAminoAcids);

            // Two isotopes
            term = _parser.ParseString("<13C><15N>ATPEILTVNSIGQLK");
            Assert.IsNull(term.Tags);
            Assert.AreEqual(2, term.GlobalModifications.Count);

            var globalMod0 = term.GlobalModifications[0].Descriptors.Single();
            Assert.AreEqual(ProFormaKey.Name, globalMod0.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, globalMod0.EvidenceType);
            Assert.AreEqual("13C", globalMod0.Value);
            var globalMod1 = term.GlobalModifications[1].Descriptors.Single();
            Assert.AreEqual(ProFormaKey.Name, globalMod1.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, globalMod1.EvidenceType);
            Assert.AreEqual("15N", globalMod1.Value);

            // Fixed protein modifications (single target)
            term = _parser.ParseString("<[MOD:01090]@C>ATPEILTCNSIGCLK");
            Assert.IsNull(term.Tags);
            Assert.AreEqual(1, term.GlobalModifications.Count);

            globalMod = term.GlobalModifications.Single().Descriptors.Single();
            Assert.AreEqual(ProFormaKey.Identifier, globalMod.Key);
            Assert.AreEqual(ProFormaEvidenceType.PsiMod, globalMod.EvidenceType);
            Assert.AreEqual("MOD:01090", globalMod.Value);
            Assert.IsNotNull(term.GlobalModifications.Single().TargetAminoAcids);
            CollectionAssert.AreEquivalent(new[] { 'C' }, term.GlobalModifications.Single().TargetAminoAcids);

            // Fixed protein modifications (multiple targets)
            term = _parser.ParseString("<[Oxidation]@C,M>MTPEILTCNSIGCLK");
            Assert.IsNull(term.Tags);
            Assert.AreEqual(1, term.GlobalModifications.Count);

            globalMod = term.GlobalModifications.Single().Descriptors.Single();
            Assert.AreEqual(ProFormaKey.Name, globalMod.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, globalMod.EvidenceType);
            Assert.AreEqual("Oxidation", globalMod.Value);
            Assert.IsNotNull(term.GlobalModifications.Single().TargetAminoAcids);
            CollectionAssert.AreEquivalent(new[] { 'C', 'M' }, term.GlobalModifications.Single().TargetAminoAcids);

            // Fixed modifications MUST be written prior to ambiguous modifications, and similar to ambiguity notation, 
            //  N-terminal modifications MUST be the last ones written, just next to the sequence.
            Assert.Throws<ProFormaParseException>(() => _parser.ParseString("[Phospho]?<[MOD:01090]@C>EM[Hydroxylation]EVTSESPEK"));
            Assert.Throws<ProFormaParseException>(() => _parser.ParseString("[Acetyl]-<[MOD:01090]@C>EM[Hydroxylation]EVTSESPEK"));
        }

        [Test]
        public void InfoTag_4_7()
        {
            // Simple info tag.
            var term = _parser.ParseString("ELV[INFO:AnyString]IS");
            Assert.AreEqual(1, term.Tags.Count);
            var desc = term.Tags.Single().Descriptors.Single();
            Assert.AreEqual(ProFormaKey.Info, desc.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, desc.EvidenceType);
            Assert.AreEqual("AnyString", desc.Value);

            // Multiple descriptors
            term = _parser.ParseString("ELVIS[Phospho|INFO:newly discovered|info:really awesome]K");
            Assert.AreEqual(1, term.Tags.Count);
            var desc0 = term.Tags.Single().Descriptors[0];
            Assert.AreEqual(ProFormaKey.Name, desc0.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, desc0.EvidenceType);
            Assert.AreEqual("Phospho", desc0.Value);
            var desc1 = term.Tags.Single().Descriptors[1];
            Assert.AreEqual(ProFormaKey.Info, desc1.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, desc1.EvidenceType);
            Assert.AreEqual("newly discovered", desc1.Value);
            var desc2 = term.Tags.Single().Descriptors[2];
            Assert.AreEqual(ProFormaKey.Info, desc2.Key);
            Assert.AreEqual(ProFormaEvidenceType.None, desc2.EvidenceType);
            Assert.AreEqual("really awesome", desc2.Value);

            // Can't use [] in info tag
            Assert.Throws<ProFormaParseException>(() => _parser.ParseString("ELVIS[Phospho|INFO:newly]discovered]K"));
        }

        [Test]
        public void JointRepresentation_4_8()
        {
            // Alternative theoretical values
            // ELVIS[U:Phospho|+79.966331]K

            // Showing both the interpretation and measured mass:
            // ELVIS[U:Phospho|Obs:+79.978]K
        }
        #endregion
    }
}