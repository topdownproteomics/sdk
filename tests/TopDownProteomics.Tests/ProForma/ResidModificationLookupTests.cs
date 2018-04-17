using NUnit.Framework;
using System.Linq;
using TopDownProteomics.Chemistry;
using TopDownProteomics.IO.Resid;
using TopDownProteomics.ProForma;
using TopDownProteomics.ProForma.Validation;
using TopDownProteomics.Tests.IO;

namespace TopDownProteomics.Tests.ProForma
{
    [TestFixture]
    public class ResidModificationLookupTests
    {
        IElementProvider _elementProvider;
        IProteoformModificationLookup _residLookup;
        ResidModification _resid38;

        [OneTimeSetUp]
        public void Setup()
        {
            _elementProvider = new MockElementProvider();

            var parser = new ResidXmlParser();
            var modifications = parser.Parse(ResidXmlParserTest.GetResidFilePath()).ToArray();

            _resid38 = modifications.Single(x => x.Id == 38);
            _residLookup = ResidModificationLookup.CreateFromModifications(new[] { _resid38 },
                _elementProvider);
        }

        [Test]
        public void DescriptorHandling()
        {
            // If the key is RESID, always handle
            Assert.IsTrue(_residLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Resid, "Anything")));
            Assert.IsTrue(_residLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Resid, "")));
            Assert.IsTrue(_residLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Resid, null)));

            // If using modification name, must end in proper ending
            Assert.IsTrue(_residLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Mod, $"Something({ProFormaKey.Resid})")));
            Assert.IsTrue(_residLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Mod, $"Something ({ProFormaKey.Resid})")));
            Assert.IsFalse(_residLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Mod, $"Something ({ProFormaKey.Resid}) ")));
            Assert.IsFalse(_residLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Mod, $"Something")));
            Assert.IsFalse(_residLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Mod, $"Something [{ProFormaKey.Resid}]")));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("Anthing")]
        public void InvalidResidIdHandling(string id)
        {
            Assert.Throws<ProteoformModificationLookupException>(
                () => _residLookup.GetModification(new ProFormaDescriptor(ProFormaKey.Resid, id)));
        }

        [Test]
        public void InvalidIntegerHandling()
        {
            ProFormaDescriptor descriptor = new ProFormaDescriptor(ProFormaKey.Resid, "abc");

            // I want this to return true and then throw an exception later.
            // This gives me an opportunity to give a meaningful error (and not just return false)

            // In this case, it is also obvious that the Resid handler was intended, so an attempt to create
            //  a modification should be made.
            Assert.True(_residLookup.CanHandleDescriptor(descriptor));
            Assert.Throws<ProteoformModificationLookupException>(() => _residLookup.GetModification(descriptor));
        }

        [Test]
        public void FindByResidId()
        {
            Assert.IsNotNull(_residLookup.GetModification(new ProFormaDescriptor(ProFormaKey.Resid, "AA0038")));
            Assert.IsNotNull(_residLookup.GetModification(new ProFormaDescriptor(ProFormaKey.Resid, "0038")));
            Assert.IsNotNull(_residLookup.GetModification(new ProFormaDescriptor(ProFormaKey.Resid, "38")));

            Assert.Throws<ProteoformModificationLookupException>(
                () => _residLookup.GetModification(new ProFormaDescriptor(ProFormaKey.Resid, "-1")));
            Assert.Throws<ProteoformModificationLookupException>(
                () => _residLookup.GetModification(new ProFormaDescriptor(ProFormaKey.Resid, "AA0000")));
            Assert.Throws<ProteoformModificationLookupException>(
                () => _residLookup.GetModification(new ProFormaDescriptor(ProFormaKey.Resid, "AA0039")));
            Assert.Throws<ProteoformModificationLookupException>(
                () => _residLookup.GetModification(new ProFormaDescriptor(ProFormaKey.Resid, "AA1234")));
        }

        [Test]
        public void FindByResidName()
        {
            Assert.IsNotNull(_residLookup.GetModification(
                new ProFormaDescriptor(ProFormaKey.Mod, "O-phospho-L-threonine(RESID)")));

            Assert.Throws<ProteoformModificationLookupException>(
                () => _residLookup.GetModification(new ProFormaDescriptor(ProFormaKey.Mod, "Something")));
            Assert.Throws<ProteoformModificationLookupException>(
                () => _residLookup.GetModification(new ProFormaDescriptor(ProFormaKey.Mod, "Something(RESID)")));
        }
    }
}