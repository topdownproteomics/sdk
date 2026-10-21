using NUnit.Framework;
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
    /// ProForma 2.0, section 4.3.2: a term can carry several labile modifications, each in its own braces, e.g.
    /// {Glycan:Hex}{Glycan:NeuAc}EMEVNESPEK. The parser kept only the last group, so every earlier labile modification was
    /// lost with no error, and the writer, the five-level classifier and the proteoform group factory never saw it.
    /// </summary>
    [TestFixture]
    public class ProFormaLabileModificationTests
    {
        private static readonly ProFormaParser Parser = new ProFormaParser();
        private static readonly ProFormaWriter Writer = new ProFormaWriter();

        [Test]
        public void TwoLabileModifications_AreBothRead()
        {
            var term = Parser.ParseString("{Glycan:Hex}{Glycan:NeuAc}EMEVNESPEK");

            Assert.AreEqual("EMEVNESPEK", term.Sequence);
            Assert.IsNotNull(term.LabileModifications);
            CollectionAssert.AreEqual(new[] { "Hex", "NeuAc" }, term.LabileModifications!.Select(g => g.Single().Value).ToArray());
            Assert.IsTrue(term.LabileModifications.All(g => g.Single().Key == ProFormaKey.Glycan));
            Assert.IsNull(term.Tags);
        }

        [Test]
        public void EachGroupIsOneModification()
        {
            var term = Parser.ParseString("{Glycan:Hex|Info:core}{Glycan:NeuAc}PEPTIDE");

            Assert.IsNotNull(term.LabileModifications);
            Assert.AreEqual(2, term.LabileModifications!.Count);
            CollectionAssert.AreEqual(new[] { "Hex", "core" }, term.LabileModifications[0].Select(d => d.Value).ToArray());
            CollectionAssert.AreEqual(new[] { "NeuAc" }, term.LabileModifications[1].Select(d => d.Value).ToArray());
            Assert.AreSame(term.LabileModifications[0], term.LabileDescriptors);
        }

        [Test]
        public void NoLabileModification_LeavesBothNull()
        {
            var term = Parser.ParseString("[Acetyl]-PEPTIDE");

            Assert.IsNull(term.LabileModifications);
            Assert.IsNull(term.LabileDescriptors);
        }

        [Test]
        public void ConstructorAndCreateFromModifications_WriteTheirGroups()
        {
            var hex = new List<ProFormaDescriptor> { new ProFormaDescriptor(ProFormaKey.Glycan, "Hex") };
            var neuAc = new List<ProFormaDescriptor> { new ProFormaDescriptor(ProFormaKey.Glycan, "NeuAc") };

            // One descriptor list is one group, written as it always was.
            var fromDescriptors = new ProFormaTerm("PEPTIDE", labileDescriptors: new List<ProFormaDescriptor> { hex[0], neuAc[0] });
            Assert.AreEqual(1, fromDescriptors.LabileModifications!.Count);
            Assert.AreEqual("{Glycan:Hex|Glycan:NeuAc}PEPTIDE", Writer.WriteString(fromDescriptors));

            var fromModifications = ProFormaTerm.CreateFromModifications("PEPTIDE",
                labileModifications: new List<IList<ProFormaDescriptor>> { hex, neuAc });
            Assert.AreSame(hex, fromModifications.LabileDescriptors);
            Assert.AreEqual("{Glycan:Hex}{Glycan:NeuAc}PEPTIDE", Writer.WriteString(fromModifications));
        }

        [TestCase("{Glycan:Hex}{Glycan:NeuAc}EMEVNESPEK")]
        [TestCase("{Glycan:Hex}{Glycan:HexNAc}{Glycan:NeuAc}PEPTIDE")]
        [TestCase("{Glycan:Hex|Info:core}{Glycan:NeuAc}PEPTIDE")]
        [TestCase("{Glycan:Hex}{Glycan:NeuAc}[Phospho]?[Acetyl][Carbamyl]-PEPTIDE")]
        [TestCase("{Glycan:Hex}{Glycan:NeuAc}EM[Oxidation]EVNES[Phospho]PEK-[Amidated]")]
        public void WriterRoundTrips(string proForma)
        {
            Assert.AreEqual(proForma, Writer.WriteString(Parser.ParseString(proForma)));
        }

        /// <summary>
        /// The classifier reads every labile group: an unidentified group anywhere makes the PTM unidentified, and two
        /// identified groups are not mistaken for one modification described two ways.
        /// </summary>
        [TestCase("{+80}{Phospho}EM[UNIMOD:35]EVEESPEK", "3")]
        [TestCase("{Phospho}{+80}EM[UNIMOD:35]EVEESPEK", "3")]
        [TestCase("{Phospho}{Acetyl}EM[UNIMOD:35]EVEESPEK", "2A")]
        public void Classifier_ReadsEveryLabileGroup(string proForma, string expectedLevel)
        {
            Assert.AreEqual(expectedLevel, FiveLevelProteoformClassifier.ClassifyProForma(Parser.ParseString(proForma), new List<string> { "0" }));
        }

        [Test]
        public void ProteoformGroupFactory_ReadsEveryLabileGroup()
        {
            var elements = new MockElementProvider();
            var factory = new ProteoformGroupFactory(elements, new IupacAminoAcidProvider(elements));
            var lookup = new BrnoModificationLookup(elements);

            var group = factory.CreateProteoformGroup(Parser.ParseString("{B:ac}{B:me1}SEQVKENCE"), lookup);

            Assert.IsNotNull(group.UnlocalizedModifications);
            Assert.AreEqual(2, group.UnlocalizedModifications!.Count);
            Assert.IsTrue(group.UnlocalizedModifications.All(m => m.IsLabile && m.Count == 1));
        }

        /// <summary>
        /// LabileDescriptors is the first group itself, not a copy, so a change made through it reaches the writer and the
        /// proteoform group factory alike.
        /// </summary>
        [Test]
        public void ClearingLabileDescriptors_ReachesTheWriterAndTheFactory()
        {
            var elements = new MockElementProvider();
            var factory = new ProteoformGroupFactory(elements, new IupacAminoAcidProvider(elements));
            var term = Parser.ParseString("{B:ac}SEQVKENCE");

            term.LabileDescriptors!.Clear();

            Assert.AreEqual("SEQVKENCE", Writer.WriteString(term));
            Assert.AreEqual(0, factory.CreateProteoformGroup(term, new BrnoModificationLookup(elements)).UnlocalizedModifications?.Count ?? 0);
        }

        [Test]
        public void AddingALabileGroup_ReachesTheWriterAndTheFactory()
        {
            var elements = new MockElementProvider();
            var factory = new ProteoformGroupFactory(elements, new IupacAminoAcidProvider(elements));
            var term = Parser.ParseString("{B:ac}SEQVKENCE");

            term.LabileModifications!.Add(new List<ProFormaDescriptor> { new ProFormaDescriptor(ProFormaKey.Name, ProFormaEvidenceType.Brno, "me1") });

            Assert.AreEqual("{B:ac}{B:me1}SEQVKENCE", Writer.WriteString(term));
            Assert.AreEqual(2, factory.CreateProteoformGroup(term, new BrnoModificationLookup(elements)).UnlocalizedModifications!.Count);
            Assert.AreSame(term.LabileModifications[0], term.LabileDescriptors);
        }

        /// <summary>A term built from one descriptor list keeps that list, as before: a later change to it is written.</summary>
        [Test]
        public void LabileDescriptorsPassedIn_StayLive()
        {
            var labile = new List<ProFormaDescriptor> { new ProFormaDescriptor(ProFormaKey.Glycan, "Hex") };
            var term = new ProFormaTerm("PEPTIDE", labileDescriptors: labile);

            labile.Add(new ProFormaDescriptor(ProFormaKey.Glycan, "NeuAc"));

            Assert.AreSame(labile, term.LabileDescriptors);
            Assert.AreEqual("{Glycan:Hex|Glycan:NeuAc}PEPTIDE", Writer.WriteString(term));
        }
    }
}
