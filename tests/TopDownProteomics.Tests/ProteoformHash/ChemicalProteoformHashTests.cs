using NUnit.Framework;
using System;
using TopDownProteomics.Biochemistry;
using TopDownProteomics.Chemistry;
using TopDownProteomics.ProForma;
using TopDownProteomics.ProForma.Validation;
using TopDownProteomics.ProteoformHash;
using TopDownProteomics.Proteomics;

namespace TopDownProteomics.Tests.ProteoformHash
{
    [TestFixture]
    public class ChemicalProteoformHashTests
    {
        IElementProvider _elementProvider;
        IResidueProvider _residueProvider;
        IProteoformModificationLookup _lookup;
        ChemicalProteoformHashGenerator _chemicalProteoformHashGenerator;

        [OneTimeSetUp]
        public void Setup()
        {
            _elementProvider = new MockElementProvider();
            _residueProvider = new IupacAminoAcidProvider(_elementProvider);
            _lookup = new CompositeModificationLookup(new IProteoformModificationLookup[]
                {
                    new FormulaLookup(_elementProvider),
                    new MassLookup(),
                    new BrnoModificationLookup(_elementProvider),
                    new IgnoreKeyModificationLookup(ProFormaKey.Info),
                });

            ProFormaParser proFormaParser = new ProFormaParser();
            ProteoformGroupFactory proteoformGroupFactory = new ProteoformGroupFactory(_elementProvider, _residueProvider);

            var mapper = new RelayAccessionMapper(d => 
            {
                if (d == "ac")
                    return Tuple.Create(ProFormaEvidenceType.PsiMod, "MOD:00394"); // acetylated residue

                return Tuple.Create(ProFormaEvidenceType.None, d);
            });

            _chemicalProteoformHashGenerator = new ChemicalProteoformHashGenerator(proFormaParser, proteoformGroupFactory, _lookup, mapper);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void BadArguments(string proForma) => Assert.Throws<ArgumentNullException>(() => _chemicalProteoformHashGenerator.Generate(proForma));

        [Test]
        [TestCase("SEQUENCE")]
        [TestCase("[Formula:C2H2O]-SEQUENCE")]
        [TestCase("SEQUENCE-[Formula:C2H2O]")]
        [TestCase("SEQ[Formula:C2H2O]UENCE")]
        [TestCase("[Formula:C2H2O]-SEQ[Formula:C2H2O]UENCE-[Formula:C2H2O]")]
        public void HashSameAsProForma(string proForma) => this.TestHash(proForma, proForma);

        [Test]
        public void MapSingleMod()
        {
            // Convert all modifications to PSI-MOD accessions
            this.TestHash("SEQUE[B:ac]NCE", "SEQUE[MOD:00394]NCE");
            this.TestHash("[B:ac]-SEQUENCE", "[MOD:00394]-SEQUENCE");
            this.TestHash("SEQUENCE-[B:ac]", "SEQUENCE-[MOD:00394]");
        }

        [Test]
        public void StripInfoTags()
        {
            string proForma = $"SEQUE[info:erase me]NCE";
            string expected = $"SEQUENCE";

            this.TestHash(proForma, expected);
        }

        [Test]
        public void ConsistentFormulas()
        {
            // Convert all modifications to PSI-MOD accessions
            this.TestHash("SEQUE[Formula:CHCHO]NCE", "SEQUE[Formula:C2H2O]NCE");
        }

        [Test]
        public void StripLeadingTrailingZeros()
        {
            // Remove leading and trailing zeros on mass modifications

            this.TestHash("SEQUE[+42.050]NCE", "SEQUE[+42.05]NCE");
            this.TestHash("SEQUE[+042.05]NCE", "SEQUE[+42.05]NCE");
            this.TestHash("SEQUE[+00042.05000]NCE", "SEQUE[+42.05]NCE");
            this.TestHash("SEQUE[-17.050]NCE", "SEQUE[-17.05]NCE");
            this.TestHash("SEQUE[-017.05]NCE", "SEQUE[-17.05]NCE");
            this.TestHash("SEQUE[-00017.05000]NCE", "SEQUE[-17.05]NCE");
        }

        private void TestHash(string proForma, string expectedHash)
        {
            IChemicalProteoformHash chemicalProteoformHash = _chemicalProteoformHashGenerator.Generate(proForma);
            Assert.AreEqual(expectedHash, chemicalProteoformHash.Hash);
            Assert.IsTrue(chemicalProteoformHash.HasProForma);
            Assert.AreEqual(expectedHash, chemicalProteoformHash.ProForma);
        }
    }
}