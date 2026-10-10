using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TopDownProteomics.Biochemistry;
using TopDownProteomics.ProForma;
using TopDownProteomics.ProForma.Validation;
using TopDownProteomics.Proteomics;
using TopDownProteomics.Tests.IO;

namespace TopDownProteomics.Tests
{
    /// <summary>
    /// ProForma 2.1, section 6.3: "Multiple modifications on the termini can be denoted with multiple modifications in
    /// square brackets", e.g. [Acetyl][Carbamyl]-QPEPTIDE and PEPTIDEG-[Methyl][Amidated]. The parser kept only the last
    /// C-terminal group, misread the first of two N-terminal groups as an unlocalized modification and then threw, read a
    /// residue after the C-terminal group into the sequence, and threw IndexOutOfRangeException on a string that is a
    /// modification with no residue.
    /// </summary>
    [TestFixture]
    public class ProFormaStackedTerminalModificationTests
    {
        private static readonly ProFormaParser Parser = new ProFormaParser();
        private static readonly ProFormaWriter Writer = new ProFormaWriter();

        private static string[] Values(IList<ProFormaDescriptor> group) => group.Select(d => d.Value).ToArray();

        [Test]
        public void TwoNTerminalModifications_AreBothRead()
        {
            var term = Parser.ParseString("[Acetyl][Carbamyl]-QPEPTIDE");

            Assert.AreEqual("QPEPTIDE", term.Sequence);
            Assert.IsNotNull(term.NTerminalModifications);
            Assert.AreEqual(2, term.NTerminalModifications!.Count);
            CollectionAssert.AreEqual(new[] { "Acetyl" }, Values(term.NTerminalModifications[0]));
            CollectionAssert.AreEqual(new[] { "Carbamyl" }, Values(term.NTerminalModifications[1]));
            Assert.IsNull(term.UnlocalizedTags);
            Assert.IsNull(term.CTerminalModifications);
        }

        [Test]
        public void TwoCTerminalModifications_AreBothRead()
        {
            var term = Parser.ParseString("PEPTIDEG-[Methyl][Amidated]");

            Assert.AreEqual("PEPTIDEG", term.Sequence);
            Assert.IsNotNull(term.CTerminalModifications);
            Assert.AreEqual(2, term.CTerminalModifications!.Count);
            CollectionAssert.AreEqual(new[] { "Methyl" }, Values(term.CTerminalModifications[0]));
            CollectionAssert.AreEqual(new[] { "Amidated" }, Values(term.CTerminalModifications[1]));
            Assert.IsNull(term.NTerminalModifications);
        }

        [Test]
        public void ThreeOnEachTerminus_KeepTheirOrder()
        {
            var term = Parser.ParseString("[UNIMOD:1][UNIMOD:5][UNIMOD:35]-PEPTIDE-[UNIMOD:2][UNIMOD:7][UNIMOD:34]");

            Assert.AreEqual("PEPTIDE", term.Sequence);
            CollectionAssert.AreEqual(new[] { "UNIMOD:1", "UNIMOD:5", "UNIMOD:35" }, term.NTerminalModifications!.Select(g => g.Single().Value));
            CollectionAssert.AreEqual(new[] { "UNIMOD:2", "UNIMOD:7", "UNIMOD:34" }, term.CTerminalModifications!.Select(g => g.Single().Value));
        }

        /// <summary>
        /// The existing single-group properties still mean what they meant: the first group. A term with one modification
        /// per terminus reads exactly as before.
        /// </summary>
        [Test]
        public void SingleGroupProperties_HoldTheFirstGroup()
        {
            var stacked = Parser.ParseString("[Acetyl][Carbamyl]-QPEPTIDE-[Methyl][Amidated]");
            CollectionAssert.AreEqual(new[] { "Acetyl" }, Values(stacked.NTerminalDescriptors!));
            CollectionAssert.AreEqual(new[] { "Methyl" }, Values(stacked.CTerminalDescriptors!));

            var single = Parser.ParseString("[Acetyl]-QPEPTIDE-[Amidated]");
            CollectionAssert.AreEqual(new[] { "Acetyl" }, Values(single.NTerminalDescriptors!));
            Assert.AreEqual(1, single.NTerminalModifications!.Count);
            Assert.AreSame(single.NTerminalDescriptors, single.NTerminalModifications[0]);
            Assert.AreEqual(1, single.CTerminalModifications!.Count);
        }

        /// <summary>
        /// A descriptor list inside one group still means one modification described several ways, and is not confused
        /// with several groups. Brackets inside a descriptor (a formula with isotopes) are not group boundaries.
        /// </summary>
        [TestCase("[Acetyl|UNIMOD:1][Carbamyl]-PEPTIDE", new[] { 2, 1 })]
        [TestCase("[Formula:[13C2]H2O][Acetyl]-PEPTIDE", new[] { 1, 1 })]
        [TestCase("[Acetyl][Formula:[13C2]H2O]-PEPTIDE", new[] { 1, 1 })]
        public void GroupsAreReadWhole(string proForma, int[] descriptorsPerGroup)
        {
            var term = Parser.ParseString(proForma);

            Assert.AreEqual("PEPTIDE", term.Sequence);
            CollectionAssert.AreEqual(descriptorsPerGroup, term.NTerminalModifications!.Select(g => g.Count));
        }

        [Test]
        public void UnlocalizedThenStackedNTerminal_AreKeptApart()
        {
            var term = Parser.ParseString("[Phospho]?[Acetyl][Carbamyl]-PEPTIDE");

            Assert.AreEqual(1, term.UnlocalizedTags!.Count);
            Assert.AreEqual(2, term.NTerminalModifications!.Count);
        }

        [Test]
        public void SeveralUnlocalized_AreStillUnlocalized()
        {
            var term = Parser.ParseString("[Phospho][Phospho]?PEPTIDE");

            Assert.AreEqual(2, term.UnlocalizedTags!.Count);
            Assert.IsNull(term.NTerminalModifications);
        }

        [Test]
        public void LabileThenStackedNTerminal_AreKeptApart()
        {
            var term = Parser.ParseString("{Glycan:Hex}[Acetyl][Carbamyl]-PEPTIDE");

            Assert.AreEqual(1, term.LabileDescriptors!.Count);
            Assert.AreEqual(2, term.NTerminalModifications!.Count);
        }

        [TestCase("[Acetyl][Carbamyl]-QPEPTIDE")]
        [TestCase("PEPTIDEG-[Methyl][Amidated]")]
        [TestCase("[UNIMOD:1][UNIMOD:5][UNIMOD:35]-PEPTIDE-[UNIMOD:2][UNIMOD:7][UNIMOD:34]")]
        [TestCase("[Phospho]?[Acetyl][Carbamyl]-PEPTIDE")]
        [TestCase("[Acetyl]-PEPTIDE-[Amidated]")]
        public void WriterRoundTrips(string proForma)
        {
            Assert.AreEqual(proForma, Writer.WriteString(Parser.ParseString(proForma)));
        }

        /// <summary>The section 4.3.1 rule both versions share: the C-terminal modification follows the last residue.</summary>
        [TestCase("PEPTIDE-[Amidated]K")]
        [TestCase("PEPTIDE-[Methyl][Amidated]K")]
        [TestCase("PEPTIDE-K")]
        public void ResidueAfterTheCTerminalModification_IsRefused(string proForma)
        {
            Assert.Throws<ProFormaParseException>(() => Parser.ParseString(proForma));
        }

        /// <summary>A modification with no residue threw IndexOutOfRangeException, which a caller catching parse errors misses.</summary>
        [TestCase("[Acetyl]")]
        [TestCase("[Acetyl][Carbamyl]")]
        [TestCase("[Acetyl]-")]
        public void ModificationWithNoResidue_IsAParseError(string proForma)
        {
            Assert.Throws<ProFormaParseException>(() => Parser.ParseString(proForma));
        }

        [Test]
        public void TerminalModificationsMustStillBeAdjacentToTheSequence()
        {
            Assert.Throws<ProFormaParseException>(() => Parser.ParseString("[Acetyl]-[Phospho]?PROTEOFORM"));
        }

        [Test]
        public void Constructor_AcceptsEitherForm_NotBoth()
        {
            var acetyl = new List<ProFormaDescriptor> { new ProFormaDescriptor("Acetyl") };
            var carbamyl = new List<ProFormaDescriptor> { new ProFormaDescriptor("Carbamyl") };

            var fromDescriptors = new ProFormaTerm("PEPTIDE", nTerminalDescriptors: acetyl);
            Assert.AreEqual(1, fromDescriptors.NTerminalModifications!.Count);

            var fromModifications = new ProFormaTerm("QPEPTIDE",
                nTerminalModifications: new List<IList<ProFormaDescriptor>> { acetyl, carbamyl });
            Assert.AreSame(acetyl, fromModifications.NTerminalDescriptors);
            Assert.AreEqual("[Acetyl][Carbamyl]-QPEPTIDE", Writer.WriteString(fromModifications));

            Assert.Throws<ArgumentException>(() => new ProFormaTerm("PEPTIDE", nTerminalDescriptors: acetyl,
                nTerminalModifications: new List<IList<ProFormaDescriptor>> { acetyl }));
            Assert.Throws<ArgumentException>(() => new ProFormaTerm("PEPTIDE", cTerminalDescriptors: acetyl,
                cTerminalModifications: new List<IList<ProFormaDescriptor>> { acetyl }));
        }

        /// <summary>
        /// A proteoform group holds one modification per terminus, so the factory refuses two, as it already refuses two
        /// different modifications described in one group.
        /// </summary>
        [Test]
        public void ProteoformGroupFactory_RefusesTwoTerminalModifications()
        {
            var elements = new MockElementProvider();
            var factory = new ProteoformGroupFactory(elements, new IupacAminoAcidProvider(elements));
            var lookup = new CompositeModificationLookup(new IProteoformModificationLookup[]
            {
                new BrnoModificationLookup(elements),
                new IgnoreKeyModificationLookup(ProFormaKey.Info)
            });
            IList<ProFormaDescriptor> Brno(string name) =>
                new List<ProFormaDescriptor> { new ProFormaDescriptor(ProFormaKey.Name, ProFormaEvidenceType.Brno, name) };

            var two = new ProFormaTerm("SEQVKENCE", nTerminalModifications: new List<IList<ProFormaDescriptor>> { Brno("ac"), Brno("me1") });
            Assert.Throws<ProteoformGroupCreateException>(() => factory.CreateProteoformGroup(two, lookup));

            var twoC = new ProFormaTerm("SEQVKENCE", cTerminalModifications: new List<IList<ProFormaDescriptor>> { Brno("ac"), Brno("me1") });
            Assert.Throws<ProteoformGroupCreateException>(() => factory.CreateProteoformGroup(twoC, lookup));

            // A second group that names no chemical modification (an Info note) does not count as a second modification.
            var withInfo = new ProFormaTerm("SEQVKENCE", nTerminalModifications: new List<IList<ProFormaDescriptor>>
            {
                Brno("ac"),
                new List<ProFormaDescriptor> { new ProFormaDescriptor(ProFormaKey.Info, "hello!") }
            });
            Assert.IsNotNull(factory.CreateProteoformGroup(withInfo, lookup).NTerminalModification);
        }

        /// <summary>The five-level classifier reads every terminal group: an unidentified second group makes the PTM unidentified.</summary>
        [Test]
        public void Classifier_ReadsEveryTerminalGroup()
        {
            var genes = new List<string> { "0" };

            string identified = FiveLevelProteoformClassifier.ClassifyProForma(Parser.ParseString("[Acetyl][Carbamyl]-QPEPTIDE"), genes);
            string unidentified = FiveLevelProteoformClassifier.ClassifyProForma(Parser.ParseString("[Acetyl][+42.011]-QPEPTIDE"), genes);

            Assert.AreNotEqual(identified, unidentified);
            Assert.AreEqual(FiveLevelProteoformClassifier.ClassifyProForma(Parser.ParseString("[+42.011]-QPEPTIDE"), genes), unidentified);
        }
    }
}
