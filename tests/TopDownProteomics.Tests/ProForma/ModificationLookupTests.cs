using NUnit.Framework;
using System.IO;
using System.Linq;
using TopDownProteomics.Chemistry;
using TopDownProteomics.Chemistry.Unimod;
using TopDownProteomics.IO.PsiMod;
using TopDownProteomics.IO.Resid;
using TopDownProteomics.IO.Unimod;
using TopDownProteomics.IO.UniProt;
using TopDownProteomics.ProForma;
using TopDownProteomics.ProForma.Validation;
using TopDownProteomics.Proteomics;
using TopDownProteomics.Tests.IO;

namespace TopDownProteomics.Tests.ProForma
{
    [TestFixture]
    public class ModificationLookupTests
    {
        private IElementProvider _elementProvider;
        private IProteoformModificationLookup _unimodLookup;
        private UnimodModification _unimod37;
        private IProteoformModificationLookup _residLookup;
        private ResidModification _resid38;
        private IProteoformModificationLookup _psiModLookup;
        private PsiModTerm _psiMod38;
        private IProteoformModificationLookup _uniProtModLookup;
        private UniprotModification _uniProtMod312;
        private IProteoformModificationLookup _formulaLookup;

        [OneTimeSetUp]
        public void Setup()
        {
            //_elementProvider = new MockElementProvider();
            NistElementParser parser = new NistElementParser();
            IElement[] elements = parser.ParseFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "elements.dat")).ToArray();
            _elementProvider = new InMemoryElementProvider(elements);

            this.SetupUnimod();
            this.SetupResid();
            this.SetupPsiMod();
            this.SetupUniProt();
            this.SetupFormula();
        }

        private void SetupUnimod()
        {
            var atomProvider = new UnimodHardCodedAtomProvider(_elementProvider);

            var parser = new UnimodOboParser();
            UnimodModification[] modifications = parser.Parse(UnimodTest.GetUnimodFilePath()).ToArray();

            _unimod37 = modifications.Single(x => x.Id == "UNIMOD:37");
            //_unimodLookup = UnimodModificationLookup.CreateFromModifications(new[] { _unimod37 },
            //    atomProvider);
            _unimodLookup = UnimodModificationLookup.CreateFromModifications(modifications, atomProvider);
        }
        private void SetupResid()
        {
            var parser = new ResidXmlParser();
            ResidModification[] modifications = parser.Parse(ResidXmlParserTest.GetResidFilePath()).ToArray();

            _resid38 = modifications.Single(x => x.Id == "AA0038");
            //_residLookup = ResidModificationLookup.CreateFromModifications(new[] { _resid38 },
            //    _elementProvider);
            _residLookup = ResidModificationLookup.CreateFromModifications(modifications,
                _elementProvider);
        }
        private void SetupPsiMod()
        {
            var parser = new PsiModOboParser();
            PsiModTerm[] modifications = parser.Parse(PsiModOboParserTest.GetFilePath()).ToArray();

            _psiMod38 = modifications.Single(x => x.Id == "MOD:00038");
            //_psiModLookup = PsiModModificationLookup.CreateFromModifications(new[] { _psiMod38 },
            //    _elementProvider);
            _psiModLookup = PsiModModificationLookup.CreateFromModifications(modifications,
                _elementProvider);
        }
        private void SetupUniProt()
        {
            var parser = new UniProtPtmListParser();
            UniprotModification[] modifications = parser.Parse(File.ReadAllText(UniProtTests.GetPtmListPath())).ToArray();

            _uniProtMod312 = modifications.Single(x => x.Id == "PTM-0312");
            //_uniProtModLookup = UniProtModificationLookup.CreateFromModifications(new[] { _uniProtMod312 },
            //    _elementProvider);
            _uniProtModLookup = UniProtModificationLookup.CreateFromModifications(modifications,
                _elementProvider);
        }
        private void SetupFormula()
        {
            _formulaLookup = new FormulaLookup(_elementProvider);
        }

        [Test]
        public void DescriptorHandling()
        {
            this.DescriptorHandling(_unimodLookup, ProFormaEvidenceType.Unimod, true);
            this.DescriptorHandling(_residLookup, ProFormaEvidenceType.Resid, false);
            this.DescriptorHandling(_psiModLookup, ProFormaEvidenceType.PsiMod, true);
            this.DescriptorHandling(_uniProtModLookup, ProFormaEvidenceType.UniProt, false);

            Assert.IsTrue(_formulaLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Formula, "Anything")));
        }
        private void DescriptorHandling(IProteoformModificationLookup lookup, ProFormaEvidenceType key, bool isDefault)
        {
            // If the key is a specific mod type, always handle
            Assert.IsTrue(lookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Name, key, "Anything")));
            Assert.IsTrue(lookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Name, key, "")));
            Assert.IsTrue(lookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Name, key, null)));

            // If using modification name, must have no ending or end in proper ending
            Assert.IsTrue(lookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Name, key, "Something")));
            Assert.IsFalse(lookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Name, ProFormaEvidenceType.Brno, "Something")));
            Assert.AreEqual(lookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Name, "Something")), isDefault);

            // This is malformed and must be interpreted as a mod "name" ... will fail when looking up modification
            //Assert.AreEqual(lookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Name, $"Something [{key}]")), isDefault);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("Anthing")]
        public void InvalidIdHandling(string id)
        {
            this.InvalidIdHandling(id, _unimodLookup, ProFormaKey.Identifier, ProFormaEvidenceType.Unimod);
            this.InvalidIdHandling(id, _residLookup, ProFormaKey.Identifier, ProFormaEvidenceType.Resid);
            this.InvalidIdHandling(id, _psiModLookup, ProFormaKey.Identifier, ProFormaEvidenceType.PsiMod);
            this.InvalidIdHandling(id, _uniProtModLookup, ProFormaKey.Identifier, ProFormaEvidenceType.UniProt);
            this.InvalidIdHandling(id, _formulaLookup, ProFormaKey.Formula);
        }
        private void InvalidIdHandling(string id, IProteoformModificationLookup lookup, ProFormaKey key,
            ProFormaEvidenceType evidenceType = ProFormaEvidenceType.None)
        {
            Assert.Throws<ProteoformModificationLookupException>(
                () => lookup.GetModification(new ProFormaDescriptor(key, evidenceType, id)));
        }

        [Test]
        public void InvalidIntegerHandling()
        {
            var descriptor = new ProFormaDescriptor(ProFormaKey.Name, ProFormaEvidenceType.Unimod, "abc");

            // I want this to return true and then throw an exception later.
            // This gives me an opportunity to give a meaningful error (and not just return false)

            // In this case, it is also obvious that the Unimod handler was intended, so an attempt to create
            //  a modification should be made.
            Assert.True(_unimodLookup.CanHandleDescriptor(descriptor));
            Assert.Throws<ProteoformModificationLookupException>(() => _unimodLookup.GetModification(descriptor));
        }

        [Test]
        public void FindById()
        {
            this.FindById(_unimodLookup, ProFormaKey.Identifier, ProFormaEvidenceType.Unimod, 37, "UNIMOD:");
            this.FindById(_residLookup, ProFormaKey.Identifier, ProFormaEvidenceType.Resid, 38, "AA");
            this.FindById(_psiModLookup, ProFormaKey.Identifier, ProFormaEvidenceType.PsiMod, 38, "MOD:");
            this.FindById(_uniProtModLookup, ProFormaKey.Identifier, ProFormaEvidenceType.UniProt, 312, "PTM-");
        }
        private void FindById(IProteoformModificationLookup lookup, ProFormaKey key, ProFormaEvidenceType evidenceType,
            int correctId, string extraPrefix)
        {
            Assert.IsNotNull(lookup.GetModification(new ProFormaDescriptor(key, evidenceType, $"{extraPrefix}{correctId}")));
            Assert.IsNotNull(lookup.GetModification(new ProFormaDescriptor(key, evidenceType, $"{correctId}")));

            Assert.Throws<ProteoformModificationLookupException>(
                () => lookup.GetModification(new ProFormaDescriptor(key, evidenceType, "-1")));
            Assert.Throws<ProteoformModificationLookupException>(
                () => lookup.GetModification(new ProFormaDescriptor(key, evidenceType, $"{extraPrefix}0")));
            Assert.Throws<ProteoformModificationLookupException>(
                () => lookup.GetModification(new ProFormaDescriptor(key, evidenceType, $"{extraPrefix}2037")));
        }

        [Test]
        public void FindByName()
        {
            this.FindByName(_unimodLookup, ProFormaEvidenceType.Unimod, true, "Trimethyl");
            this.FindByName(_residLookup, ProFormaEvidenceType.Resid, false, "O-phospho-L-threonine");
            this.FindByName(_psiModLookup, ProFormaEvidenceType.PsiMod, false, "3-hydroxy-L-proline");
            this.FindByName(_uniProtModLookup, ProFormaEvidenceType.UniProt, false, "(2-aminosuccinimidyl)acetic acid (Asp-Gly)");
        }
        private void FindByName(IProteoformModificationLookup lookup, ProFormaEvidenceType evidenceType,
            bool isDefault, string correctName)
        {
            if (isDefault)
                Assert.IsNotNull(lookup.GetModification(new ProFormaDescriptor(ProFormaKey.Name, ProFormaEvidenceType.None, correctName)));

            Assert.IsNotNull(lookup.GetModification(new ProFormaDescriptor(ProFormaKey.Name, evidenceType, correctName)));

            Assert.Throws<ProteoformModificationLookupException>(
                () => lookup.GetModification(new ProFormaDescriptor(ProFormaKey.Name, ProFormaEvidenceType.None, "Something")));
            Assert.Throws<ProteoformModificationLookupException>(
                () => lookup.GetModification(new ProFormaDescriptor(ProFormaKey.Name, evidenceType, "Something")));
        }

        [Test]
        public void FormulaLookup()
        {
            string formulaString = "C2H2O";
            ProFormaDescriptor proFormaDescriptor = new ProFormaDescriptor(ProFormaKey.Formula, formulaString);
            ChemicalFormula chemicalFormula = new ChemicalFormula(new IEntityCardinality<IElement>[]
            {
                new EntityCardinality<IElement>(_elementProvider.GetElement("C"), 2),
                new EntityCardinality<IElement>(_elementProvider.GetElement("H"), 2),
                new EntityCardinality<IElement>(_elementProvider.GetElement("O"), 1),
            });

            IProteoformModification proteoformModification = _formulaLookup.GetModification(proFormaDescriptor);
            Assert.AreEqual(chemicalFormula, proteoformModification.GetChemicalFormula());
        }

        [Test]
        public void PsiModIsotope()
        {
            var parser = new PsiModOboParser();
            PsiModTerm[] modifications = parser.Parse(PsiModOboParserTest.GetFilePath()).ToArray();

            PsiModTerm psiMod402 = modifications.Single(x => x.Id == "MOD:00402");
            IProteoformModificationLookup psiModLookup = PsiModModificationLookup.CreateFromModifications(new[] { psiMod402 },
                _elementProvider);
            this.FindById(psiModLookup, ProFormaKey.Identifier, ProFormaEvidenceType.PsiMod, 402, "MOD:");

            IProteoformModification mod = psiModLookup.GetModification(new ProFormaDescriptor(ProFormaKey.Identifier,
                ProFormaEvidenceType.PsiMod, "MOD:00402"));

            ChemicalFormula chemicalFormula = new ChemicalFormula(
                new IEntityCardinality<IElement>[]
                {
                    new EntityCardinality<IElement>(_elementProvider.GetElement("C"), 22),
                    new EntityCardinality<IElement>(_elementProvider.GetElement("H", 1), 30),
                    new EntityCardinality<IElement>(_elementProvider.GetElement("H", 2), 8),
                    new EntityCardinality<IElement>(_elementProvider.GetElement("N"), 4),
                    new EntityCardinality<IElement>(_elementProvider.GetElement("O"), 6),
                    new EntityCardinality<IElement>(_elementProvider.GetElement("S"), 1),
                });
            Assert.IsTrue(chemicalFormula.Equals(mod.GetChemicalFormula()));
        }
    }
}