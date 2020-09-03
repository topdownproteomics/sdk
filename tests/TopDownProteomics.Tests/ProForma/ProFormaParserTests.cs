using NUnit.Framework;
using System;
using System.Linq;
using TopDownProteomics.ProForma;

namespace TopDownProteomics.Tests
{
    [TestFixture]
    public class ProFormaParserTests
    {
        public static ProFormaParser _parser = new ProFormaParser(allowLegacySyntax: true);

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
            // Tweaking for v2

            const string proFormaString = "SEQUEN[Methyl|+14.02]CE";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("SEQUENCE", term.Sequence);
            Assert.IsNotNull(term.Tags);
            Assert.AreEqual(1, term.Tags.Count);

            ProFormaTag tag = term.Tags.Single();
            Assert.AreEqual(5, tag.ZeroBasedIndex);
            Assert.AreEqual(2, tag.Descriptors.Count);

            Assert.AreEqual(ProFormaKey.KnownModificationName, tag.Descriptors[0].Key);
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
            Assert.AreEqual(ProFormaKey.KnownModificationName, term.Tags.Single().Descriptors.Single().Key);
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
            Assert.AreEqual(5, tag.ZeroBasedIndex);
            Assert.AreEqual(2, tag.Descriptors.Count);

            Assert.AreEqual(ProFormaKey.KnownModificationName, tag.Descriptors.First().Key);
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
            Assert.AreEqual(ProFormaKey.KnownModificationName, cTerm.Key);
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
            Assert.AreEqual(2, tag1.ZeroBasedIndex);
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
            Assert.AreEqual(ProFormaKey.KnownModificationName, tag1.Descriptors.Single().Key);
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
            Assert.AreEqual(ProFormaKey.KnownModificationName, nTerm.Key);
            Assert.AreEqual("Acetyl", nTerm.Value);

            ProFormaTag tag1 = term.Tags[0];
            Assert.AreEqual(0, tag1.ZeroBasedIndex);
            Assert.AreEqual(2, tag1.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.KnownModificationName, tag1.Descriptors.First().Key);
            Assert.AreEqual("Phospho", tag1.Descriptors.First().Value);
            Assert.AreEqual(ProFormaKey.Mass, tag1.Descriptors.Last().Key);
            Assert.AreEqual("+79.966331", tag1.Descriptors.Last().Value);

            ProFormaTag tag5 = term.Tags[1];
            Assert.AreEqual(4, tag5.ZeroBasedIndex);
            Assert.AreEqual(3, tag5.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.KnownModificationName, tag5.Descriptors[0].Key);
            Assert.AreEqual("Acetyl", tag5.Descriptors[0].Value);
            Assert.AreEqual(ProFormaKey.Unimod, tag5.Descriptors[1].Key);
            Assert.AreEqual("UNIMOD:1", tag5.Descriptors[1].Value);
            Assert.AreEqual(ProFormaKey.Mass, tag5.Descriptors[2].Key);
            Assert.AreEqual("+42.010565", tag5.Descriptors[2].Value);

            ProFormaTag tag120 = term.Tags[2];
            Assert.AreEqual(119, tag120.ZeroBasedIndex);
            Assert.AreEqual(1, tag120.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.Unimod, tag120.Descriptors.Single().Key);
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
            // Tweaking for v2

            const string proFormaString = "MTLFQLREHWFVYKDDEKLTAFRNK[p-adenosine|R:N6-(phospho-5'-adenosine)-L-lysine| RESID:AA0227| MOD:00232| N6AMPLys]SMLFQRELRPNEEVTWK";
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("MTLFQLREHWFVYKDDEKLTAFRNKSMLFQRELRPNEEVTWK", term.Sequence);
            Assert.IsNotNull(term.Tags);
            Assert.AreEqual(1, term.Tags.Count);
            Assert.IsNull(term.NTerminalDescriptors);
            Assert.IsNull(term.CTerminalDescriptors);

            ProFormaTag tag25 = term.Tags[0];
            Assert.AreEqual(24, tag25.ZeroBasedIndex);
            Assert.AreEqual(5, tag25.Descriptors.Count);
            Assert.AreEqual(ProFormaKey.KnownModificationName, tag25.Descriptors[0].Key);
            Assert.AreEqual("p-adenosine", tag25.Descriptors[0].Value);
            Assert.AreEqual(ProFormaKey.Resid, tag25.Descriptors[1].Key);
            Assert.AreEqual("N6-(phospho-5'-adenosine)-L-lysine", tag25.Descriptors[1].Value);
            Assert.AreEqual(ProFormaKey.Resid, tag25.Descriptors[2].Key);
            Assert.AreEqual("AA0227", tag25.Descriptors[2].Value);
            Assert.AreEqual(ProFormaKey.PsiMod, tag25.Descriptors[3].Key);
            Assert.AreEqual("MOD:00232", tag25.Descriptors[3].Value);
            Assert.AreEqual(ProFormaKey.KnownModificationName, tag25.Descriptors[4].Key);
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

        // List for meeting
        //  * Double check using modification names w/o prefixes (4.2.1)
        //  * RESID uses AA0000 for identifiers (4.2.2)

        //  * What does pipe mean?
        //    - For info tags, it means more information about the tag
        //    - For ambiguity groups, it allows one to put a group name (THIS ONE seems closer to XLs)
        //    - For joint representation of data, it means you are using multiple pieces of information in the same tag
        
        //  * Do both sides of a crosslink always have the same ID?

        //  * Future direction -> Metal bindings?
        //  * Can we have ranges and ambiguity groups together? e.g. EM[Oxidation]EV(ST)[#g1]S[#g1]ES[Phospho|#g1]PEK
        //  * What does this mean? EM[Oxidation]EVT[#g1]S[#g1]ES[Acetyl|Trimethyl|#g1]PEK
        //    - Do both modifications apply to the group?
        //  * What does this mean? EM[Oxidation]EVT[#g1]S[#g1]ES[Acetyl|#g1|info:found trimethyl here in pub XYZ]PEK
        //    - How does it work if someone wants to write information about the whole group? or just one part?


        // Questions for Ryan
        //  ? Should I expose all functionality using interfaces or hide some details in specific classes?
        //  ? Why would one want to syntax parse and not go on to Proteoform Group?
        //    1. Filter/append and write back out
        //  ? Can XLs and ambiguity groups use the same mechanism?

        // Term
        //  Sequence
        //  N-Term LIST
        //  C-Term LIST
        //  Tags
        //  Labile LIST
        //  Unlocalized LIST
        //  AmbiguityGroup LIST
        //  Fixed Modifications LIST

        // TagGroup
        //  Name
        //  Key, Value (? ONLY 1 pair for the whole group?)
        //  MembershipDescriptor LIST
        //  Required membership count (so you could say, e.g., you need 2 out of 3)
        //  IsAmbiguous()

        // MembershipDescriptor
        //  Index (Start and End to support ranges in groups?)
        //  Membership Weight
        //  Type -> Fixed, Ambiguous

        //////// AmbiguityGroup
        ////////  Name
        ////////  Tag? Descriptor? LIST?

        //////// AmbiguityGroupTag
        ////////  Index
        ////////  Score

        // Tag
        //  Start Index (Range support)
        //  End Index

        // Descriptor
        //  Key, Value
        //  -- Crosslink Name (need this here if I do grouping like above?) --
        //  Evidence Type: Unknown, Theoretical, Observed

        // Fixed Modification
        //  Type -> AA, Element
        //  Target LIST -> only needed for AA
        //  Descriptor -> any type for AA, should probably be Formula for element

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

            Assert.AreEqual(ProFormaKey.KnownModificationName, desc1.Key);
            Assert.AreEqual(ProFormaKey.KnownModificationName, desc2.Key);
        }

        [Test]
        public void ModificationNameUsage_4_2_1_Prefixes()
        {
            // RESID is R:
            var term = _parser.ParseString("EM[R:Methionine sulfone]EVEES[O-phospho-L-serine]PEK");
            var desc1 = term.Tags[0].Descriptors.Single();
            var desc2 = term.Tags[1].Descriptors.Single();

            Assert.AreEqual(ProFormaKey.Resid, desc1.Key);
            Assert.AreEqual(ProFormaKey.KnownModificationName, desc2.Key);

            // XL-MOD is X:
            term = _parser.ParseString("EMEVTK[X:DSS#XL1]SESPEK");
            desc1 = term.Tags[0].Descriptors.Single();

            Assert.AreEqual(ProFormaKey.XlMod, desc1.Key);

            // GNO is G:
            term = _parser.ParseString("NEEYN[G:G59626AS]K");
            desc1 = term.Tags[0].Descriptors.Single();

            Assert.AreEqual(ProFormaKey.Gno, desc1.Key);
        }

        // TODO: 4.2.1.1 -> Validation, not parsing

        [Test]
        [TestCase("EM[MOD:00719]EVEES[MOD:00046]PEK", ProFormaKey.PsiMod)]
        [TestCase("EM[UNIMOD:15]EVEES[UNIMOD:56]PEK", ProFormaKey.Unimod)]
        [TestCase("EM[RESID:AA0581]EVEES[RESID:AA0037]PEK", ProFormaKey.Resid)]
        public void ModificationAccessionNumbers_4_2_2(string proFormaString, ProFormaKey modType)
        {
            var term = _parser.ParseString(proFormaString);

            Assert.AreEqual("EMEVEESPEK", term.Sequence);
            Assert.AreEqual(2, term.Tags.Count);

            var desc1 = term.Tags[0].Descriptors.Single();
            var desc2 = term.Tags[1].Descriptors.Single();

            Assert.AreEqual(modType, desc1.Key);
            Assert.AreEqual(modType, desc2.Key);
        }

        [Test]
        public void Crosslinkers_XL_MOD_4_2_3()
        {
            // Add cross link name to descriptor.

            // Using the XL-MOD CV, arbitrary suffixes MUST be used to denote links between two residues.
            // EMEVTK[XLMOD:02001#XL1]SESPEK[#XL1]
            // EMK[XLMOD:02000#XL1]EVTK[XLMOD:02001#XL2]SESK[#XL1]PEK[#XL2]

            // "Dead end" crosslinks
            // EMEVTK[XLMOD:02001#XL1]SESPEK
            // EMEVTK[XLMOD:02001]SESPEK

            // Inter-chain crosslinks
            // SEK[XLMOD:02001#XL1]UENCE\\EMEVTK[XLMOD:02001#XL1]SESPEK
            // SEK[XLMOD:02001#XL1]UENCE\\EMEVTK[#XL1]SESPEK

            // Disulfides
            // EVTSEKC[MOD:00034#XL1]LEMSC[#XL1]EFD
            // EVTSEKC[L-cystine (cross-link)#XL1]LEMSC[#XL1]EFD
            // EVTSEKC[X:Disulfide#XL1]LEMSC[#XL1]EFD
        }

        [Test]
        public void Glycans_GNO_MOD_4_2_4()
        {
            // Use standard descriptor.

            // NEEYN[GNO:G59626AS]K
            // YPVLN[GNO:G62765YT]VTMPN[GNO:G02815KT]NSNGKFDK
        }

        [Test]
        public void DeltaMassNotation_4_2_5()
        {
            // Add evidence type to descriptor to handle prefixes.

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
            // Parse straight, consider some validation change (e.g. force a mass to be specified, etc.)

            // RTAAX[+367.0537]WT
        }

        [Test]
        public void ChemicalFormulas_4_2_7()
        {
            // Use standard descriptor, add parser.

            // SEQUEN[Formula:C12H20O2]CE
            // SEQUEN[Formula:C12 H20 O2]CE
            // SEQUEN[Formula:HN-1O2]CE
            // SEQUEN[Formula:[13C2][12C-2]H2N]CE
            // SEQUEN[Formula:[13C2]C-2H2N]CE
        }

        [Test]
        public void GlycanComposition_4_2_8()
        {
            // Use standard descriptor, add parser.

            // SEQUEN[Glycan:HexNAc1Hex2]CE
        }

        [Test]
        public void TerminalModifications_4_3_1()
        {
            // Use standard descriptor

            // [iTRAQ4plex]-EM[Hydroxylation]EVNES[Phospho]PEK
            // [iTRAQ4plex]-EM[U:Hydroxylation]EVNES[Phospho]PEK[iTRAQ4plex]-[Methyl]

            // TODO: Add check for using negative delta mass ... might interfere with the dash notation
        }

        [Test]
        public void LabileModifications_4_3_2()
        {
            // Add labile descriptor list to term

            // {Hex}EM[U:Hydroxylation]EVNES[Phospho]PEK[iTRAQ4plex]
            // {Hex}[iTRAQ4plex]-EM[Hydroxylation]EVNES[Phospho]PEK[iTRAQ4plex]
            // {Hex}[iTRAQ4plex]-EM[Hydroxylation]EVNES[Phospho]PEK[iTRAQ4plex]-[Methyl]
        }

        [Test]
        public void Ambiguity_UnknownPosition_4_4_1()
        {
            // Use unlocalized list on term.

            // [Phospho]?EM[Hydroxylation]EVTSESPEK
            // [Phospho][Phospho]?[Acetyl]-EM[Hydroxylation]EVTSESPEK
            // [Phospho]^2[Methyl]?[Acetyl]-EM[Hydroxylation]EVTSESPEK

            // INVALID [Acetyl]-[Phospho]^2?EM[Hydroxylation]EVTSESPEK
        }

        [Test]
        public void Ambiguity_PossiblePositions_4_4_2()
        {
            // Use TagGroup List on term.

            // This is read as a named group 'g1' indicates that a phosphorylation exists on either T5, S6 or S8
            // EM[Oxidation]EVT[#g1]S[#g1]ES[Phospho#g1]PEK

            // The following example is not valid because a single preferred location must be chosen for a modification:
            // INVALID EM[Oxidation]EVT[#g1]S[Phospho#g1]ES[Phospho#g1]PEK
        }

        [Test]
        public void Ambiguity_Ranges_4_4_3()
        {
            // Use start and end indexes on Tag

            // Ranges of amino acids as possible locations for the modifications may be represented using parentheses within the amino acid sequence.
            // PROT(EOSFORMS)[+19.0523]ISK
            // PROT(EOC[Carbamidomethyl]FORMS)[+19.0523]ISK

            // Overlapping ranges represent a more complex case and are not yet supported, and so, the following example would NOT be valid:
            // INVALID P(ROT(EOSFORMS)[+19.0523]IS)[+19.0523]K


            // Ranges + scores
            // PROT(EOSFORMS)[+19.0523#g1(0.01)]ISK
            // PROT(EOC[Carbamidomethyl]FORMS)[+19.05233#g1(0.09)]ISK
        }

        [Test]
        public void Ambiguity_PossiblePositionsWithScores_4_4_4()
        {
            // Use MembershipDescriptors to store scores.

            // The values of the modification localization scores can be indicated in parentheses within the same group and brackets. 
            // EM[Oxidation]EVT[#g1(0.01)]S[#g1(0.09)]ES[Phospho#g1(0.90)]PEK

            // The additional option to represent localisation scores is to leave the position of the modification as unknown using the ‘?’ notation, 
            //  but report the localization modification scores at specific sites.
            // [Phospho#s1]?EM[Oxidation]EVT[#s1(0.01)]S[#s1(0.90)]ES[#s1(0.90)]PEK
        }

        [Test]
        public void NoMultipleModificationsSameSite_4_5()
        {
            // TODO: Example?
        }

        [Test]
        public void GlobalModifications_4_6()
        {
            // Use Fixed Modification LIST on term

            // Representation of isotopes
            // <13C>ATPEILTVNSIGQLK
            // <15N>ATPEILTVNSIGQLK
            // <D>ATPEILTVNSIGQLK
            // <13C><15N>ATPEILTVNSIGQLK

            // Fixed protein modifications
            // <[S-carboxamidomethyl-L-cysteine]@C>ATPEILTCNSIGCLK
            // <[MOD:01090]@C>ATPEILTCNSIGCLK
            // <[Oxidation]@C,M>MTPEILTCNSIGCLK

            // Fixed modifications MUST be written prior to ambiguous modifications, and similar to ambiguity notation, N-terminal modifications MUST be the last ones written, just next to the sequence. 
            // INVALID: [Phospho]?<[MOD:01090]@C>EM[Hydroxylation]EVTSESPEK
            // INVALID: [Acetyl]-<[MOD:01090]@C>EM[Hydroxylation]EVTSESPEK
        }

        [Test]
        public void InfoTag_4_7()
        {
            // Use standard descriptor.

            // ELV[INFO:AnyString]IS
            // ELVIS[Phospho|INFO:newly discovered]K
            // ELVIS[Phospho|INFO:newly discovered|INFO:really awesome]K
            // INVALID: ELVIS[Phospho|INFO:newly]discovered]K
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