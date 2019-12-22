using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using TopDownProteomics.Chemistry;

namespace TopDownProteomics.Tests
{
    [TestFixture]
    public class NistElementParserTest
    {
        [Test]
        public void LoadEntireFile()
        {
            var parser = new NistElementParser();
            var elements = parser.ParseFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "elements.dat"));

            Assert.IsNotNull(elements);
            Assert.AreEqual(84, elements.Count); // Only count elements with isotope abundances

            var h = elements.Single(x => x.AtomicNumber == 1);
            Assert.AreEqual("H", h.Symbol);
            Assert.AreEqual(2, h.Isotopes.Count);
            var h1 = h.Isotopes.Single(x => x.AtomicMass < 2);
            Assert.AreEqual(1.00782503223, h1.AtomicMass);
            Assert.AreEqual(0.999885, h1.RelativeAbundance);
            var h2 = h.Isotopes.Single(x => x.AtomicMass > 2);
            Assert.AreEqual(2.01410177812, h2.AtomicMass);
            Assert.AreEqual(0.000115, h2.RelativeAbundance);

            var c = elements.Single(x => x.AtomicNumber == 6);
            var c12 = c.Isotopes.Single(x => Math.Round(x.AtomicMass) == 12);
            Assert.AreEqual(6, c12.NeutronCount);
            var c13 = c.Isotopes.Single(x => Math.Round(x.AtomicMass) == 13);
            Assert.AreEqual(7, c13.NeutronCount);

            var ru = elements.Single(x => x.AtomicNumber == 44);
            Assert.AreEqual("Ru", ru.Symbol);
            Assert.AreEqual(7, ru.Isotopes.Count);

            var xe = elements.Single(x => x.AtomicNumber == 54);
            Assert.AreEqual("Xe", xe.Symbol);
            Assert.AreEqual(9, xe.Isotopes.Count);

            var u = elements.Single(x => x.AtomicNumber == 92);
            Assert.AreEqual("U", u.Symbol);
            Assert.AreEqual(3, u.Isotopes.Count);
        }
    }
}