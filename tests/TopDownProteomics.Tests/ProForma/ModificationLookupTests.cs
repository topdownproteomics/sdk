using NUnit.Framework;
using System.Linq;
using TopDownProteomics.Chemistry;
using TopDownProteomics.IO.PsiMod;
using TopDownProteomics.IO.Resid;
using TopDownProteomics.IO.Unimod;
using TopDownProteomics.IO.UniProt;
using TopDownProteomics.ProForma;
using TopDownProteomics.ProForma.Validation;
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

        [OneTimeSetUp]
        public void Setup()
        {
            _elementProvider = new MockElementProvider();

            this.SetupUnimod();
            this.SetupResid();
            this.SetupPsiMod();
            this.SetupUniProt();
        }

        private void SetupUnimod()
        {
            var atomProvider = new MockUnimodCompositionAtomProvider(_elementProvider);

            var parser = new UnimodOboParser();
            UnimodModification[] modifications = parser.Parse(UnimodTest.GetUnimodFilePath()).ToArray();

            _unimod37 = modifications.Single(x => x.Id == 37);
            _unimodLookup = UnimodModificationLookup.CreateFromModifications(new[] { _unimod37 },
                atomProvider);
        }
        private void SetupResid()
        {
            var parser = new ResidXmlParser();
            ResidModification[] modifications = parser.Parse(ResidXmlParserTest.GetResidFilePath()).ToArray();

            _resid38 = modifications.Single(x => x.Id == 38);
            _residLookup = ResidModificationLookup.CreateFromModifications(new[] { _resid38 },
                _elementProvider);
        }
        private void SetupPsiMod()
        {
            var parser = new PsiModParser();
            PsiModTerm[] modifications = parser.Parse(PsiModParserTest.GetFilePath()).ToArray();

            _psiMod38 = modifications.Single(x => x.Id == 38);
            _psiModLookup = PsiModModificationLookup.CreateFromModifications(new[] { _psiMod38 },
                _elementProvider);
        }
        private void SetupUniProt()
        {
            var parser = new UniProtPtmListParser();
            UniprotModification[] modifications = parser.Parse(UniProtTests.Get_PtmList()).ToArray();

            _uniProtMod312 = modifications.Single(x => x.Id == 312);
            _uniProtModLookup = UniProtModificationLookup.CreateFromModifications(new[] { _uniProtMod312 },
                _elementProvider);
        }

        [Test]
        public void DescriptorHandling()
        {
            this.DescriptorHandling(_unimodLookup, ProFormaKey.Unimod, true);
            this.DescriptorHandling(_residLookup, ProFormaKey.Resid, false);
            this.DescriptorHandling(_psiModLookup, ProFormaKey.PsiMod, false);
            this.DescriptorHandling(_uniProtModLookup, ProFormaKey.UniProt, false);
        }
        private void DescriptorHandling(IProteoformModificationLookup lookup, string key, bool isDefault)
        {
            // If the key is a specific mod type, always handle
            Assert.IsTrue(lookup.CanHandleDescriptor(new ProFormaDescriptor(key, "Anything")));
            Assert.IsTrue(lookup.CanHandleDescriptor(new ProFormaDescriptor(key, "")));
            Assert.IsTrue(lookup.CanHandleDescriptor(new ProFormaDescriptor(key, null)));

            // If using modification name, must have no ending or end in proper ending
            Assert.IsTrue(lookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Mod, $"Something({key})")));
            Assert.IsTrue(lookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Mod, $"Something ({key})")));
            Assert.IsFalse(lookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Mod, $"Something (Other_Type) ")));
            Assert.AreEqual(lookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Mod, $"Something")), isDefault);

            // This is malformed and must be interpreted as a mod "name" ... will fail when looking up modification
            Assert.AreEqual(lookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Mod, $"Something [{key}]")), isDefault);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("Anthing")]
        public void InvalidIdHandling(string id)
        {
            this.InvalidIdHandling(id, _unimodLookup, ProFormaKey.Unimod);
            this.InvalidIdHandling(id, _residLookup, ProFormaKey.Resid);
            this.InvalidIdHandling(id, _psiModLookup, ProFormaKey.PsiMod);
            this.InvalidIdHandling(id, _uniProtModLookup, ProFormaKey.UniProt);
        }
        private void InvalidIdHandling(string id, IProteoformModificationLookup lookup, string key)
        {
            Assert.Throws<ProteoformModificationLookupException>(
                () => _unimodLookup.GetModification(new ProFormaDescriptor(ProFormaKey.Unimod, id)));
        }

        [Test]
        public void InvalidIntegerHandling()
        {
            var descriptor = new ProFormaDescriptor(ProFormaKey.Unimod, "abc");

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
            this.FindById(_unimodLookup, ProFormaKey.Unimod, 37, "UNIMOD:");
            this.FindById(_residLookup, ProFormaKey.Resid, 38, "AA");
            this.FindById(_psiModLookup, ProFormaKey.PsiMod, 38, "MOD:");
            this.FindById(_uniProtModLookup, ProFormaKey.UniProt, 312, "PTM-");
        }
        private void FindById(IProteoformModificationLookup lookup, string key, int correctId, string extraPrefix)
        {
            Assert.IsNotNull(lookup.GetModification(new ProFormaDescriptor(key, $"{extraPrefix}{correctId}")));
            Assert.IsNotNull(lookup.GetModification(new ProFormaDescriptor(key, $"{correctId}")));

            Assert.Throws<ProteoformModificationLookupException>(
                () => lookup.GetModification(new ProFormaDescriptor(key, "-1")));
            Assert.Throws<ProteoformModificationLookupException>(
                () => lookup.GetModification(new ProFormaDescriptor(key, $"{extraPrefix}0")));
            Assert.Throws<ProteoformModificationLookupException>(
                () => lookup.GetModification(new ProFormaDescriptor(key, $"1025")));
            Assert.Throws<ProteoformModificationLookupException>(
                () => lookup.GetModification(new ProFormaDescriptor(key, $"{extraPrefix}2037")));
        }

        [Test]
        public void FindByName()
        {
            this.FindByName(_unimodLookup, ProFormaKey.Unimod, true, "Trimethyl");
            this.FindByName(_residLookup, ProFormaKey.Resid, false, "O-phospho-L-threonine");
            this.FindByName(_psiModLookup, ProFormaKey.PsiMod, false, "3-hydroxy-L-proline");
            this.FindByName(_uniProtModLookup, ProFormaKey.UniProt, false, "(2-aminosuccinimidyl)acetic acid (Asp-Gly)");
        }
        private void FindByName(IProteoformModificationLookup lookup, string key, bool isDefault, string correctName)
        {
            if (isDefault)
                Assert.IsNotNull(lookup.GetModification(new ProFormaDescriptor(ProFormaKey.Mod, correctName)));

            Assert.IsNotNull(lookup.GetModification(new ProFormaDescriptor(ProFormaKey.Mod, $"{correctName}({key})")));

            Assert.Throws<ProteoformModificationLookupException>(
                () => lookup.GetModification(new ProFormaDescriptor(ProFormaKey.Mod, "Something")));
            Assert.Throws<ProteoformModificationLookupException>(
                () => lookup.GetModification(new ProFormaDescriptor(ProFormaKey.Mod, $"Something({key})")));
        }
    }
}