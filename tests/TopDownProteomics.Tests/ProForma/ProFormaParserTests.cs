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
            Assert.AreEqual(2, term.Tags.Single().ZeroBasedIndex);
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
            const string proFormaString = "SEQUEN[mod:Methyl|mass:+14.02]CE";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("SEQUENCE", term.Sequence);
            Assert.IsNotNull(term.Tags);
            Assert.AreEqual(1, term.Tags.Count);

            ProFormaTag tag = term.Tags.Single();
            Assert.AreEqual(5, tag.ZeroBasedIndex);
            Assert.AreEqual(2, tag.Descriptors.Count);

            Assert.AreEqual(ProFormaKey.Mod, tag.Descriptors[0].Key);
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
            Assert.AreEqual(2, term.Tags.Single().ZeroBasedIndex);
            Assert.AreEqual(1, term.Tags.Single().Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Mod, term.Tags.Single().Descriptors.Single().Key);
            Assert.AreEqual(modName, term.Tags.Single().Descriptors.Single().Value);
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
            Assert.AreEqual(5, tag.ZeroBasedIndex);
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
            Assert.AreEqual(0, tag80.ZeroBasedIndex);
            Assert.AreEqual(1, tag80.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Mass, tag80.Descriptors.Single().Key);
            Assert.AreEqual("80", tag80.Descriptors.Single().Value);

            ProFormaTag tag14 = term.Tags[1];
            Assert.AreEqual(4, tag14.ZeroBasedIndex);
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
            Assert.AreEqual(2, tag1.ZeroBasedIndex);
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

        public void PossibleSiteAmbiguityRules()
        {
            const string proFormaString = "PROT[Phospho|#eg]EOS[#eg]FORMS[#eg]";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("PROTEOSFORMS", term.Sequence);
            Assert.IsNotNull(term.Tags);
            Assert.AreEqual(3, term.Tags.Count);
            Assert.IsNull(term.NTerminalDescriptors);
            Assert.IsNull(term.CTerminalDescriptors);

            ProFormaTag tag1 = term.Tags[0];
            Assert.AreEqual(3, tag1.ZeroBasedIndex);
            Assert.AreEqual(2, tag1.Descriptors.Count);
            Assert.AreEqual(1, tag1.Descriptors.OfType<ProFormaAmbiguityDescriptor>().Count());
            Assert.AreEqual(ProFormaAmbiguityAffix.PossibleSite, tag1.Descriptors.OfType<ProFormaAmbiguityDescriptor>().Single().Affix);
            Assert.AreEqual("eg", tag1.Descriptors.OfType<ProFormaAmbiguityDescriptor>().Single().Group);

            ProFormaTag tag2 = term.Tags[1];
            Assert.AreEqual(6, tag2.ZeroBasedIndex);
            Assert.AreEqual(1, tag2.Descriptors.Count);
            Assert.AreEqual(ProFormaAmbiguityAffix.PossibleSite, tag2.Descriptors.Single().Key);
            Assert.AreEqual(ProFormaAmbiguityAffix.PossibleSite, (tag2.Descriptors.Single() as ProFormaAmbiguityDescriptor).Affix);
            Assert.AreEqual("eg", tag2.Descriptors.Single().Value);
            Assert.AreEqual("eg", (tag2.Descriptors.Single() as ProFormaAmbiguityDescriptor).Group);
        }

        public void RangeAmbiguityRules()
        {
            const string proFormaString = "PROT[mass:19|A->]EOSFORMS[<-A]";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("PROTEOSFORMS", term.Sequence);
            Assert.IsNotNull(term.Tags);
            Assert.AreEqual(2, term.Tags.Count);
            Assert.IsNull(term.NTerminalDescriptors);
            Assert.IsNull(term.CTerminalDescriptors);

            ProFormaTag tag1 = term.Tags[0];
            Assert.AreEqual(3, tag1.ZeroBasedIndex);
            Assert.AreEqual(2, tag1.Descriptors.Count);
            Assert.AreEqual(1, tag1.Descriptors.OfType<ProFormaAmbiguityDescriptor>().Count());
            Assert.AreEqual(ProFormaAmbiguityAffix.LeftBoundary, tag1.Descriptors.OfType<ProFormaAmbiguityDescriptor>().Single().Affix);
            Assert.AreEqual("A", tag1.Descriptors.OfType<ProFormaAmbiguityDescriptor>().Single().Group);

            ProFormaTag tag2 = term.Tags[1];
            Assert.AreEqual(11, tag2.ZeroBasedIndex);
            Assert.AreEqual(1, tag2.Descriptors.Count);
            Assert.AreEqual(ProFormaAmbiguityAffix.RightBoundary, tag2.Descriptors.Single().Key);
            Assert.AreEqual(ProFormaAmbiguityAffix.RightBoundary, (tag2.Descriptors.Single() as ProFormaAmbiguityDescriptor).Affix);
            Assert.AreEqual("A", tag2.Descriptors.Single().Value);
            Assert.AreEqual("A", (tag2.Descriptors.Single() as ProFormaAmbiguityDescriptor).Group);
        }

        public void UnlocalizedAmbiguityRules()
        {
            const string proFormaString = "[Phospho]?PROTEOSFORMS";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("PROTEOSFORMS", term.Sequence);
            Assert.IsNull(term.Tags);
            Assert.IsNotNull(term.UnlocalizedTags);
            Assert.AreEqual(1, term.UnlocalizedTags.Count);
            Assert.IsNull(term.NTerminalDescriptors);
            Assert.IsNull(term.CTerminalDescriptors);

            ProFormaTag tag1 = term.UnlocalizedTags[0];
            Assert.AreEqual(-1, tag1.ZeroBasedIndex);
            Assert.AreEqual(1, tag1.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Mod, tag1.Descriptors.Single().Key);
            Assert.AreEqual("Phospho", tag1.Descriptors.Single().Value);
        }

        [Test]
        [TestCase("[Acetyl]-[Phospho]?PROTEOFORM")] // terminal mods must be adjacent to sequence
        [TestCase("PROT[Phospho|#]EOFORMS[#]")] // empty group string
        [TestCase("PROT[Phospho|->]EOFORMS[<-]")] // empty group string
        public void AmbiguityRulesInvalid(string proFormaString)
        {
            Assert.Throws<ProFormaParseException>(() => _parser.ParseString(proFormaString));
        }

        [Test]
        public void BestPractice_i()
        {
            const string proFormaString = "[Acetyl]-S[Phospho|mass:79.966331]GRGK[Acetyl|Unimod:1|mass:42.010565]QGGKARAKAKTRSSRAGLQFPVGRVHRLLRKGNYAERVGAGAPVYLAAVLEYLTAEILELAGNAARDNKKTRIIPRHLQLAIRNDEELNKLLGKVTIAQGGVLPNIQAVLLPKKT[Unimod:21]ESHHKAKGK";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("SGRGKQGGKARAKAKTRSSRAGLQFPVGRVHRLLRKGNYAERVGAGAPVYLAAVLEYLTAEILELAGNAARDNKKTRIIPRHLQLAIRNDEELNKLLGKVTIAQGGVLPNIQAVLLPKKTESHHKAKGK", term.Sequence);
            Assert.IsNotNull(term.Tags);
            Assert.AreEqual(3, term.Tags.Count);
            Assert.IsNotNull(term.NTerminalDescriptors);
            Assert.AreEqual(1, term.NTerminalDescriptors.Count);
            Assert.IsNull(term.CTerminalDescriptors);

            var nTerm = term.NTerminalDescriptors[0];
            Assert.AreEqual(ProFormaKey.Mod, nTerm.Key);
            Assert.AreEqual("Acetyl", nTerm.Value);

            ProFormaTag tag1 = term.Tags[0];
            Assert.AreEqual(0, tag1.ZeroBasedIndex);
            Assert.AreEqual(2, tag1.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Mod, tag1.Descriptors.First().Key);
            Assert.AreEqual("Phospho", tag1.Descriptors.First().Value);
            Assert.AreEqual(ProFormaKey.Mass, tag1.Descriptors.Last().Key);
            Assert.AreEqual("79.966331", tag1.Descriptors.Last().Value);

            ProFormaTag tag5 = term.Tags[1];
            Assert.AreEqual(4, tag5.ZeroBasedIndex);
            Assert.AreEqual(3, tag5.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Mod, tag5.Descriptors[0].Key);
            Assert.AreEqual("Acetyl", tag5.Descriptors[0].Value);
            Assert.AreEqual(ProFormaKey.Unimod, tag5.Descriptors[1].Key);
            Assert.AreEqual("1", tag5.Descriptors[1].Value);
            Assert.AreEqual(ProFormaKey.Mass, tag5.Descriptors[2].Key);
            Assert.AreEqual("42.010565", tag5.Descriptors[2].Value);

            ProFormaTag tag120 = term.Tags[2];
            Assert.AreEqual(119, tag120.ZeroBasedIndex);
            Assert.AreEqual(1, tag120.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Unimod, tag120.Descriptors.Single().Key);
            Assert.AreEqual("21", tag120.Descriptors.Single().Value);
        }

        [Test]
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
            Assert.AreEqual(ProFormaKey.Unimod, nTerm.Key);
            Assert.AreEqual("1", nTerm.Value);

            ProFormaTag tag1 = term.Tags[0];
            Assert.AreEqual(0, tag1.ZeroBasedIndex);
            Assert.AreEqual(1, tag1.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Unimod, tag1.Descriptors.Single().Key);
            Assert.AreEqual("21", tag1.Descriptors.Single().Value);

            ProFormaTag tag5 = term.Tags[1];
            Assert.AreEqual(4, tag5.ZeroBasedIndex);
            Assert.AreEqual(1, tag5.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Unimod, tag5.Descriptors.Single().Key);
            Assert.AreEqual("1", tag5.Descriptors.Single().Value);

            ProFormaTag tag44 = term.Tags[2];
            Assert.AreEqual(43, tag44.ZeroBasedIndex);
            Assert.AreEqual(1, tag44.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Unimod, tag44.Descriptors.Single().Key);
            Assert.AreEqual("21", tag44.Descriptors.Single().Value);
        }

        [Test]
        public void BestPractice_iii()
        {
            const string proFormaString = "MTLFQLREHWFVYKDDEKLTAFRNK[p-adenosine| N6-(phospho-5'-adenosine)-L-lysine(RESID)| RESID:AA0227| PSI-MOD:MOD:00232| N6AMPLys(PSI-MOD)]SMLFQRELRPNEEVTWK";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("MTLFQLREHWFVYKDDEKLTAFRNKSMLFQRELRPNEEVTWK", term.Sequence);
            Assert.IsNotNull(term.Tags);
            Assert.AreEqual(1, term.Tags.Count);
            Assert.IsNull(term.NTerminalDescriptors);
            Assert.IsNull(term.CTerminalDescriptors);

            ProFormaTag tag25 = term.Tags[0];
            Assert.AreEqual(24, tag25.ZeroBasedIndex);
            Assert.AreEqual(5, tag25.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Mod, tag25.Descriptors[0].Key);
            Assert.AreEqual("p-adenosine", tag25.Descriptors[0].Value);
            Assert.AreEqual(ProFormaKey.Mod, tag25.Descriptors[1].Key);
            Assert.AreEqual(" N6-(phospho-5'-adenosine)-L-lysine(RESID)", tag25.Descriptors[1].Value);
            Assert.AreEqual(ProFormaKey.Resid, tag25.Descriptors[2].Key);
            Assert.AreEqual("AA0227", tag25.Descriptors[2].Value);
            Assert.AreEqual(ProFormaKey.PsiMod, tag25.Descriptors[3].Key);
            Assert.AreEqual("MOD:00232", tag25.Descriptors[3].Value);
            Assert.AreEqual(ProFormaKey.Mod, tag25.Descriptors[4].Key);
            Assert.AreEqual(" N6AMPLys(PSI-MOD)", tag25.Descriptors[4].Value);
        }

        [Test]
        public void BestPractice_iv()
        {
            const string proFormaString = "MTLFQLDEKLTA[mass:-37.995001|info:unknown modification]FRNKSMLFQRELRPNEEVTWK";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("MTLFQLDEKLTAFRNKSMLFQRELRPNEEVTWK", term.Sequence);
            Assert.IsNotNull(term.Tags);
            Assert.AreEqual(1, term.Tags.Count);
            Assert.IsNull(term.NTerminalDescriptors);
            Assert.IsNull(term.CTerminalDescriptors);

            ProFormaTag tag12 = term.Tags[0];
            Assert.AreEqual(11, tag12.ZeroBasedIndex);
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
        [TestCase("PROTEOFXRM")]
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
        public void ModificationNameUsage_4_2_1()
        {
            const string proFormaString = "EM[Oxidation]EVEES[U:Phospho]PEK";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual(proFormaString, term.Sequence);
            Assert.AreEqual(2, term.Tags.Count);

            // EM[L-methionine sulfoxide]EVEES[O-phospho-L-serine]PEK
            // EM[Oxidation]EVE[Cation:Mg[II]]ES[Phospho]PEK
        }

        // TODO: 4.2.1.1 -> Validation, not parsing

        [Test]
        public void ModificationAccessionNumbers_4_2_2()
        {
            const string proFormaString = "EM[MOD:00719]EVEES[MOD:00046]PEK";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual(proFormaString, term.Sequence);
            Assert.AreEqual(2, term.Tags.Count);

            // EM[UNIMOD:15]EVEES[UNIMOD:56]PEK
        }

        [Test]
        public void Crosslinkers_XL_MOD_4_2_3()
        {
            // EMEVTK[XLMOD:02001.XL1]SESPEK[XLMOD:02001.XL1]
            // EMK[XLMOD:02000.XL1]EVTK[XLMOD:02001.XL2]SESK[XLMOD:02000.XL1]PEK[XLMOD:02001.XL2]

        }

        [Test]
        public void Glycans_GNO_MOD_4_2_4()
        {
            // NEEYN[GNO:G59626AS]K
            // YPVLN[GNO:G62765YT]VTMPN[GNO:G02815KT]NSNGKFDK
        }

        [Test]
        public void DeltaMassNotation_4_2_5()
        {
            // No prefixes
            // EM[+15.9949]EVEES[+79.9663]PEK
            // EM[+15.995]EVEES[+79.966]PEK

            // Prefixes
            // TODO: One of these should not validate because these are theoretical masses.
            // EM[U:+15.9949]EVEES[U:+79.9663]PEK
            // EM[U:+15.995]EVEES[U:+79.966]PEK
            // EM[U:+15.995]EVEES[Obs:+79.978]PEK
        }

        [Test]
        public void GapOfKnownMass_4_2_6()
        {
            // RTAAX[+367.0537]WT
        }

        [Test]
        public void ChemicalFormulas_4_2_7()
        {
            // SEQUEN[Formula:C12H20O2]CE
            // SEQUEN[Formula:HN-1O2]CE
            // SEQUEN[Formula:[13]C2[12]C-2H2N]CE
            // SEQUEN[Formula:CH3(CH2)4CH3]CE
        }

        [Test]
        public void GlycanComposition_4_2_8()
        {
            // SEQUEN[Glycan:Hex2Man]CE
            // SEQUEN[Glycan:HexNAc]CE
        }

        [Test]
        public void TerminalModifications_4_3_1()
        {
            // [iTRAQ4plex]-EM[Hydroxylation]EVNES[Phospho]PEK
            // [iTRAQ4plex]-EM[U:Hydroxylation]EVNES[Phospho]PEK[iTRAQ4plex]-[Methyl]
        }

        [Test]
        public void LabileModifications_4_3_2()
        {
            // {Hex}EM[U:Hydroxylation]EVNES[Phospho]PEK[iTRAQ4plex]
            // {Hex}[iTRAQ4plex]-EM[Hydroxylation]EVNES[Phospho]PEK[iTRAQ4plex]
            // {Hex}[iTRAQ4plex]-EM[Hydroxylation]EVNES[Phospho]PEK[iTRAQ4plex]-[Methyl]
        }

        [Test]
        public void Ambiguity_UnknownPosition_4_4_1()
        {
            // [Phospho]?EM[Hydroxylation]EVTSESPEK
            // [Phospho][Phospho]?[Acetyl]-EM[Hydroxylation]EVTSESPEK
            // [Phospho]*2?[Acetyl]-EM[Hydroxylation]EVTSESPEK

            // INVALID [Acetyl]-[Phospho]*2? EM[Hydroxylation]EVTSESPEK
        }

        [Test]
        public void Ambiguity_PossiblePositions_4_4_2()
        {
            // This is read as a named group 'g1' indicates that a phosphorylation exists on either T5, S6 or S8
            // EM[Oxidation]EVT[#g1]S[#g1]ES[Phospho|#g1]PEK
        }

        [Test]
        public void Ambiguity_PossiblePositionsWithScores_4_4_3()
        {
            // The values of the modification localization scores can be indicated in parentheses within the same group and brackets. 
            // EM[Oxidation]EVT[#g1(0.01)]S[#g1(0.09)]ES[Phospho|#g1(0.90)]PEK

            // Similarly, modification position preference may be specified in boolean notation (case insensitive). 
            // In the following example, the last site is preferred. 
            // TODO: Multiple sites may be listed as preferred.

            // EM[Oxidation]EVT[#g1(False)]S[#g1(False)]ES[Phospho|#g1(True)]PEK

            // The third option to represent localisation scores is to leave the position of the modification as unknown using the ‘?’ notation, 
            //  but report the localization modification scores at specific sites.
            // [Phospho|#s1]?EM[Oxidation]EVT[#s1(0.01)]S[#s1(0.90)]ES[#s1(0.90)]PEK
        }

        [Test]
        public void Ambiguity_Ranges_4_4_4()
        {
            // Ranges of amino acids as possible locations for the modifications may be represented using parentheses within the amino acid sequence.
            // PROT(EOSFORMS)[+19.0523]ISK
            // PROT(EOC[Carbamidomethyl]FORMS)[+19.0523]ISK
        }

        [Test]
        public void NoMultipleModificationsSameSite_4_5()
        {
            // TODO: Example?
        }

        [Test]
        public void GlobalModifications_4_6()
        {
            // Representation of isotopes
            // <13C>ATPEILTVNSIGQLK
            // <15N>ATPEILTVNSIGQLK
            // <D>ATPEILTVNSIGQLK
            // <13C><15N>ATPEILTVNSIGQLK

            // Fixed protein modifications
            // <[S-carboxamidomethyl-L-cysteine]@C>ATPEILTCNSIGCLK
            // <[MOD:01090]@C>ATPEILTCNSIGCLK

            // Fixed modifications MUST be written prior to ambiguous modifications, and similar to ambiguity notation, N-terminal modifications MUST be the last ones written, just next to the sequence. 
            // INVALID: [Phospho]?<[MOD:01090]@C> EM[Hydroxylation]EVTSESPEK
            // INVALID: [Acetyl]-<[MOD:01090]@C> EM[Hydroxylation]EVTSESPEK
        }

        [Test]
        public void InfoTag_4_7()
        {
            // ELV[INFO:AnyString]IS
            // ELVIS[Phospho|INFO:newly discovered]K
            // ELVIS[Phospho|INFO:newly discovered|INFO:really awesome]K
            // INVALID: ELVIS[Phospho|INFO:newly]discovered]K
        }

        // TODO: 4.8 is up in the air a bit, wait to implement
        #endregion
    }
}