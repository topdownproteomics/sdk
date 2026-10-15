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
            Assert.IsNotNull(term.LabileDescriptors);
            CollectionAssert.AreEqual(new[] { "Hex", "NeuAc" }, term.LabileDescriptors!.Select(d => d.Value).ToArray());
            Assert.IsTrue(term.LabileDescriptors.All(d => d.Key == ProFormaKey.Glycan));
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
            CollectionAssert.AreEqual(new[] { "Hex", "core", "NeuAc" }, term.LabileDescriptors!.Select(d => d.Value).ToArray());
        }

        [Test]
        public void NoLabileModification_LeavesBothNull()
        {
            var term = Parser.ParseString("[Acetyl]-PEPTIDE");

            Assert.IsNull(term.LabileModifications);
            Assert.IsNull(term.LabileDescriptors);
        }

        [Test]
        public void Constructor_AcceptsEitherForm_NotBoth()
        {
            var hex = new List<ProFormaDescriptor> { new ProFormaDescriptor(ProFormaKey.Glycan, "Hex") };
            var neuAc = new List<ProFormaDescriptor> { new ProFormaDescriptor(ProFormaKey.Glycan, "NeuAc") };

            // One descriptor list is one group, written as it always was.
            var fromDescriptors = new ProFormaTerm("PEPTIDE", labileDescriptors: new List<ProFormaDescriptor> { hex[0], neuAc[0] });
            Assert.AreEqual(1, fromDescriptors.LabileModifications!.Count);
            Assert.AreEqual("{Glycan:Hex|Glycan:NeuAc}PEPTIDE", Writer.WriteString(fromDescriptors));

            var fromModifications = new ProFormaTerm("PEPTIDE", labileModifications: new List<IList<ProFormaDescriptor>> { hex, neuAc });
            Assert.AreEqual(2, fromModifications.LabileDescriptors!.Count);
            Assert.AreEqual("{Glycan:Hex}{Glycan:NeuAc}PEPTIDE", Writer.WriteString(fromModifications));

            Assert.Throws<System.ArgumentException>(() => new ProFormaTerm("PEPTIDE", labileDescriptors: hex,
                labileModifications: new List<IList<ProFormaDescriptor>> { hex }));
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
    }
}
