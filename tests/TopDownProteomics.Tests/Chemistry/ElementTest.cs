using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDownProteomics.Chemistry;

namespace TopDownProteomics.Tests.Chemistry
{
    [TestFixture]
    public class ElementTest
    {
        [Test]
        public void Equality()
        {
            Element h = new Element(1, "H", new ReadOnlyCollection<IIsotope>(new[]
            {
                new Isotope(1.00782503223, 0, 0.999885),
                new Isotope(2.01410177812, 1, 0.000115)
            }));
            Element anotherH = new Element(1, "H", new ReadOnlyCollection<IIsotope>(new[]
            {
                new Isotope(1.00782503223, 0, 0.999885),
                new Isotope(2.01410177812, 1, 0.000115)
            }));

            Element h1 = new Element(1, "H", new ReadOnlyCollection<IIsotope>(new[]
            {
                new Isotope(1.00782503223, 0, 1)
            }));
            Element anotherH1 = new Element(1, "H", new ReadOnlyCollection<IIsotope>(new[]
            {
                new Isotope(1.00782503223, 0, 1)
            }));

            Element h2 = new Element(1, "H", new ReadOnlyCollection<IIsotope>(new[]
            {
                new Isotope(2.01410177812, 1, 1)
            }));
            Element anotherH2 = new Element(1, "H", new ReadOnlyCollection<IIsotope>(new[]
            {
                new Isotope(2.01410177812, 1, 1)
            }));

            Assert.IsTrue(h.Equals(h));
            Assert.IsTrue(h.Equals(anotherH));
            Assert.IsTrue(h1.Equals(h1));
            Assert.IsTrue(h1.Equals(anotherH1));
            Assert.IsTrue(h2.Equals(h2));
            Assert.IsTrue(h2.Equals(anotherH2));

            Assert.IsFalse(h.Equals(h1));
            Assert.IsFalse(h.Equals(h2));
            Assert.IsFalse(h1.Equals(h));
            Assert.IsFalse(h1.Equals(h2));
            Assert.IsFalse(h2.Equals(h));
            Assert.IsFalse(h2.Equals(h1));
        }
    }
}
