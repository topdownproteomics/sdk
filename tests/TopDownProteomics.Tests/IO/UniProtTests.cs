using NUnit.Framework;
using System.IO;
using System.Linq;
using TopDownProteomics.IO.UniProt;

namespace TopDownProteomics.Tests.IO
{
    [TestFixture]
    public class UniProtTests
    {
        public static string GetPtmListPath() => Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ptmlist.txt");

        [Test]
        public void BasicModificationFileParse()
        {
            var parser = new UniProtPtmListParser();
            var entries = parser.Parse(File.ReadAllText(GetPtmListPath())).ToList();

            Assert.AreEqual(676, entries.Count);

            // Make sure we parse first and last
            var mod0 = entries[0];
            var mod675 = entries[675];

            Assert.AreEqual("(2-aminosuccinimidyl)acetic acid (Asn-Gly)", mod0.Name);
            Assert.AreEqual("S-linked (HexNAc...) cysteine", mod675.Name);

            // Confirm missing Resid, Unimod, and PSI-MOD mappings
            Assert.IsNull(mod675.Resid);
            Assert.IsNull(mod675.Unimod);
            Assert.IsNull(mod675.PsiMod);

            UniprotModification mod3 = entries[3];
            Assert.AreEqual("(3R)-3-hydroxyasparagine", mod3.Identifier);
            Assert.AreEqual("PTM-0369", mod3.Accession);
            Assert.AreEqual(UniprotFeatureType.ModifiedResidue, mod3.FeatureKey);
            Assert.AreEqual("Asparagine", mod3.Target);
            Assert.AreEqual("Amino acid side chain", mod3.AminoAcidPosition);
            Assert.AreEqual("Anywhere", mod3.PolypeptidePosition);
            Assert.AreEqual("O1", mod3.CorrectionFormula);
            Assert.AreEqual(15.994915, mod3.MonoisotopicMassDifference);
            Assert.AreEqual(16.00, mod3.AverageMassDifference);
            Assert.AreEqual("Intracellular localisation or Extracellular and lumenal localisation", mod3.CellularLocation);

            Assert.AreEqual(1, mod3.TaxonomicRanges.Count);
            Assert.AreEqual("Eukaryota; taxId:40674 (Mammalia)", mod3.TaxonomicRanges.Single());
            Assert.AreEqual(1, mod3.Keywords.Count);
            Assert.AreEqual("Hydroxylation", mod3.Keywords.Single());

            Assert.AreEqual("AA0026", mod3.Resid);
            Assert.AreEqual("MOD:00035", mod3.PsiMod);
            Assert.AreEqual("35", mod3.Unimod);
        }
    }
}