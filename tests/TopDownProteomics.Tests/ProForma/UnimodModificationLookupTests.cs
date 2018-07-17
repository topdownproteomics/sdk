using NUnit.Framework;
using System.Linq;
using TopDownProteomics.Chemistry;
using TopDownProteomics.IO.Unimod;
using TopDownProteomics.ProForma;
using TopDownProteomics.ProForma.Validation;
using TopDownProteomics.Tests.IO;

namespace TopDownProteomics.Tests.ProForma
{
    [TestFixture]
    public class UnimodModificationLookupTests
    {
        private IElementProvider _elementProvider;
        private IProteoformModificationLookup _unimodLookup;
        private UnimodModification _unimod37;

        [OneTimeSetUp]
        public void Setup()
        {
            _elementProvider = new MockElementProvider();
            var atomProvider = new MockUnimodCompositionAtomProvider(_elementProvider);

            var parser = new UnimodOboParser();
            UnimodModification[] modifications = parser.Parse(UnimodTest.GetUnimodFilePath()).ToArray();

            _unimod37 = modifications.Single(x => x.Id == 37);
            _unimodLookup = UnimodModificationLookup.CreateFromModifications(new[] { _unimod37 },
                atomProvider);
        }

        [Test]
        public void DescriptorHandling()
        {
            // If the key is Unimod, always handle
            Assert.IsTrue(_unimodLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Unimod, "Anything")));
            Assert.IsTrue(_unimodLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Unimod, "")));
            Assert.IsTrue(_unimodLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Unimod, null)));

            // If using modification name, must have no ending or end in proper ending
            Assert.IsTrue(_unimodLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Mod, $"Something({ProFormaKey.Unimod})")));
            Assert.IsTrue(_unimodLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Mod, $"Something ({ProFormaKey.Unimod})")));
            Assert.IsFalse(_unimodLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Mod, $"Something ({ProFormaKey.Resid}) ")));
            Assert.IsTrue(_unimodLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Mod, $"Something")));

            // This is malformed and must be interpreted as a mod "name" ... will fail when looking up modification
            Assert.IsTrue(_unimodLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Mod, $"Something [{ProFormaKey.Unimod}]")));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("Anthing")]
        public void InvalidIdHandling(string id)
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
            Assert.IsNotNull(_unimodLookup.GetModification(new ProFormaDescriptor(ProFormaKey.Unimod, "UNIMOD:37")));
            Assert.IsNotNull(_unimodLookup.GetModification(new ProFormaDescriptor(ProFormaKey.Unimod, "37")));

            Assert.Throws<ProteoformModificationLookupException>(
                () => _unimodLookup.GetModification(new ProFormaDescriptor(ProFormaKey.Unimod, "-1")));
            Assert.Throws<ProteoformModificationLookupException>(
                () => _unimodLookup.GetModification(new ProFormaDescriptor(ProFormaKey.Unimod, "UNIMOD:0")));
            Assert.Throws<ProteoformModificationLookupException>(
                () => _unimodLookup.GetModification(new ProFormaDescriptor(ProFormaKey.Unimod, "UNIMOD:1025")));
            Assert.Throws<ProteoformModificationLookupException>(
                () => _unimodLookup.GetModification(new ProFormaDescriptor(ProFormaKey.Unimod, "UNIMOD:2037")));
        }

        [Test]
        public void FindByName()
        {
            Assert.IsNotNull(_unimodLookup.GetModification(new ProFormaDescriptor(ProFormaKey.Mod, "Trimethyl")));
            Assert.IsNotNull(_unimodLookup.GetModification(new ProFormaDescriptor(ProFormaKey.Mod, $"Trimethyl({ProFormaKey.Unimod})")));

            Assert.Throws<ProteoformModificationLookupException>(
                () => _unimodLookup.GetModification(new ProFormaDescriptor(ProFormaKey.Mod, "Something")));
            Assert.Throws<ProteoformModificationLookupException>(
                () => _unimodLookup.GetModification(new ProFormaDescriptor(ProFormaKey.Mod, $"Something({ProFormaKey.Unimod})")));
        }
    }
}