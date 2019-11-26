using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TopDownProteomics.Chemistry;

namespace TopDownProteomics.Tests.Chemistry
{
    public class ChemicalFormulaTest
    {
        IElementProvider _elementProvider;

        [OneTimeSetUp]
        public void Setup()
        {
            _elementProvider = new MockElementProvider();
        }

        [Test]
        public void ParseTest()
        {
            string formulaString = "H(2) O";

            bool success = ChemicalFormula.TryParseString(formulaString, _elementProvider, out ChemicalFormula chemicalFormula);

            Assert.IsTrue(success);
            Assert.IsNotNull(chemicalFormula);

            IReadOnlyCollection<IEntityCardinality<IElement>> elements = chemicalFormula.GetElements();
            Assert.AreEqual(2, elements.Count);

            IEntityCardinality<IElement> h = elements.SingleOrDefault(e => e.Entity.Symbol == "H");
            Assert.IsNotNull(h);
            Assert.AreEqual(2, h.Count);

            IEntityCardinality<IElement> o = elements.SingleOrDefault(e => e.Entity.Symbol == "O");
            Assert.IsNotNull(o);
            Assert.AreEqual(1, o.Count);

            Assert.AreEqual(ChemicalFormula.Water(_elementProvider), chemicalFormula);
        }

        [Test]
        public void IncorrectFormatTest()
        {
            string formulaString = "H2O";
            Assert.IsFalse(ChemicalFormula.TryParseString(formulaString, _elementProvider, out ChemicalFormula chemicalFormula));
        }
    }
}