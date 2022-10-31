using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using TopDownProteomics.Biochemistry;
using TopDownProteomics.Chemistry;
using TopDownProteomics.Chemistry.Unimod;
using TopDownProteomics.IO.Unimod;
using TopDownProteomics.IO.UniProt;
using TopDownProteomics.ProForma;
using TopDownProteomics.ProForma.Validation;
using TopDownProteomics.ProteoformHash;
using TopDownProteomics.Proteomics;
using TopDownProteomics.Tests.IO;

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
            var unimodOboParser = new UnimodOboParser();
            UnimodModification[] modifications = unimodOboParser.Parse(UnimodTest.GetUnimodFilePath()).ToArray();

            NistElementParser nistParser = new NistElementParser();
            IElement[] elements = nistParser.ParseFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "elements.dat")).ToArray();
            _elementProvider = new InMemoryElementProvider(elements);

            _residueProvider = new IupacAminoAcidProvider(_elementProvider);
            var atomProvider = new UnimodHardCodedAtomProvider(_elementProvider);

            _lookup = new CompositeModificationLookup(new IProteoformModificationLookup[]
                {
                    UnimodModificationLookup.CreateFromModifications(modifications, atomProvider),
                    new FormulaLookup(_elementProvider),
                    new GlycanCompositionLookup(new HardCodedGlycanResidueProvider(_elementProvider)),
                    new MassLookup(),
                    new BrnoModificationLookup(_elementProvider),
                    new IgnoreKeyModificationLookup(ProFormaKey.Info),
                });

            ProFormaParser proFormaParser = new ProFormaParser();
            ProteoformGroupFactory proteoformGroupFactory = new ProteoformGroupFactory(_elementProvider, _residueProvider);

            var mapper = new RelayAccessionMapper(d =>
            {
                if (d == "B:ac")
                    return Tuple.Create(ProFormaEvidenceType.PsiMod, "MOD:00394"); // acetylated residue

                return Tuple.Create(ProFormaEvidenceType.None, d);
            });

            var parser = new UniProtPtmListParser();
            var entries = parser.Parse(File.ReadAllText(UniProtTests.GetPtmListPath())).ToList();

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
        [TestCase("[Glycan:Hex2HexNAc]-SEQ[Glycan:Hex2HexNAc]UENCE-[Glycan:Hex2HexNAc]")]
        public void HashSameAsProForma(string proForma) => this.TestHash(proForma, proForma);

        [Test]
        public void MapSingleMod()
        {
            this.TestHash("SEQUE[B:ac]NCE", "SEQUE[MOD:00394]NCE");                 // Tag
            this.TestHash("[B:ac]-SEQUENCE", "[MOD:00394]-SEQUENCE");               // N-term
            this.TestHash("SEQUENCE-[B:ac]", "SEQUENCE-[MOD:00394]");               // C-term
            this.TestHash("{B:ac}SEQUENCE", "{MOD:00394}SEQUENCE");                 // Labile
            this.TestHash("[B:ac]?SEQUENCE", "[MOD:00394]?SEQUENCE");               // Unlocalized
            this.TestHash("SE[B:ac#g1]QUE[#g1]NCE", "SE[MOD:00394#g1]QUE[#g1]NCE"); // Tag group
            this.TestHash("S(EQUE)[B:ac]NCE", "S(EQUE)[MOD:00394]NCE");             // Range
            this.TestHash("<[B:ac]@E>SEQUENCE", "<[MOD:00394]@E>SEQUENCE");         // Global
        }

        // Sent question to PSI-MOD about which generic mod to use. I think it is MOD:00394
        //[Test]
        //public void MappingUnimodToGenericPsiMod()
        //{
        //    this.TestHash("SEQUE[UNIMOD:1]NCE", "SEQUE[MOD:00394]NCE");
        //}

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
            // Merge duplicate elements
            this.TestHash("SEQUE[Formula:CHCHO]NCE", "SEQUE[Formula:C2H2O]NCE");

            // Handle negatives
            this.TestHash("SEQUE[Formula:CH-1]NCE", "SEQUE[Formula:CH-1]NCE");
        }

        [Test]
        public void ConsistentCompositions()
        {
            // Merge duplicate elements
            this.TestHash("SEQUE[Glycan:HexHexNAcHex]NCE", "SEQUE[Glycan:Hex2HexNAc]NCE");

            // Check order of elements (should be alpha)
            this.TestHash("SEQUE[Glycan:HexdHex3]NCE", "SEQUE[Glycan:dHex3Hex]NCE");

            // Handle negatives
            this.TestHash("SEQUE[Glycan:HexdHex-2]NCE", "SEQUE[Glycan:dHex-2Hex]NCE");
        }

        [Test]
        public void StripLeadingTrailingZeros()
        {
            // Remove leading and ensure 4 decimal places on mass modifications

            this.TestHash("SEQUE[+42.050]NCE", "SEQUE[+42.0500]NCE");
            this.TestHash("SEQUE[+042.05]NCE", "SEQUE[+42.0500]NCE");
            this.TestHash("SEQUE[+00042.05000]NCE", "SEQUE[+42.0500]NCE");
            this.TestHash("SEQUE[-17.050]NCE", "SEQUE[-17.0500]NCE");
            this.TestHash("SEQUE[-017.05]NCE", "SEQUE[-17.0500]NCE");
            this.TestHash("SEQUE[-00017.05000]NCE", "SEQUE[-17.0500]NCE");
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