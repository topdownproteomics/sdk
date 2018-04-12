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
        ResidModification _resid38;

        [OneTimeSetUp]
        public void Setup()
        {
            _elementProvider = new MockElementProvider();

            var parser = new ResidXmlParser();
            var modifications = parser.Parse(ResidXmlParserTest.GetResidFilePath()).ToArray();

            _resid38 = modifications.Single(x => x.Id == 38);
        }

        [Test]
        public void DescriptorHandling()
        {
            IProteoformModificationLookup residLookup = ResidModificationLookup.CreateFromModifications(new[] { _resid38 },
                _elementProvider);

            // With and without and prefix is OK
            Assert.IsTrue(residLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Resid, "AA0038")));
            Assert.IsTrue(residLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Resid, "0038")));
            Assert.IsTrue(residLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Resid, "38")));

            // Try some invalid arguments
            Assert.IsFalse(residLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Resid, "")));
            Assert.IsFalse(residLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Resid, null)));

            // If using modification name, must end in proper ending
            Assert.IsTrue(residLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Mod, $"Something({ProFormaKey.Resid})")));
            Assert.IsTrue(residLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Mod, $"Something ({ProFormaKey.Resid})")));
            Assert.IsFalse(residLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Mod, $"Something")));
            Assert.IsFalse(residLookup.CanHandleDescriptor(new ProFormaDescriptor(ProFormaKey.Mod, $"Something [{ProFormaKey.Resid}]")));
        }

        [Test]
        public void InvalidIntegerHandling()
        {
            IProteoformModificationLookup residLookup = ResidModificationLookup.CreateFromModifications(new[] { _resid38 },
                _elementProvider);

            ProFormaDescriptor descriptor = new ProFormaDescriptor(ProFormaKey.Resid, "abc");

            // I want this to return true and then throw an exception later.
            // This gives me an opportunity to give a meaningful error (and not just return false)
            Assert.True(residLookup.CanHandleDescriptor(descriptor));
            Assert.Throws<ProteoformModificationLookupException>(() => residLookup.GetModification(descriptor));
        }
    }
}
