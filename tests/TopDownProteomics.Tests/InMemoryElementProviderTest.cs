using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDownProteomics.Chemistry;

namespace TopDownProteomics.Tests
{
    [TestFixture]
    public class InMemoryElementProviderTest
    {
        [Test]
        public void TwoElements()
        {
            IElement[] elements = new Element[]
            {
                new Element(1, "H", null),
                new Element(2, "He", null)
            };

            InMemoryElementProvider provider = new InMemoryElementProvider(elements);

            Assert.AreEqual(elements[0], provider.GetElement(1));
            Assert.AreEqual(elements[1], provider.GetElement(2));
            Assert.AreEqual(elements[0], provider.GetElement("H"));
            Assert.AreEqual(elements[1], provider.GetElement("He"));
        }

        [Test]
        public void MissingElements()
        {
            IElement[] elements = new Element[]
{
                new Element(1, "H", null),
                new Element(6, "C", null)
};

            InMemoryElementProvider provider = new InMemoryElementProvider(elements);

            Assert.AreEqual(elements[0], provider.GetElement(1));
            Assert.AreEqual(elements[1], provider.GetElement(6));
            Assert.AreEqual(elements[0], provider.GetElement("H"));
            Assert.AreEqual(elements[1], provider.GetElement("C"));
        }
    }
}
