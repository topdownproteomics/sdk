using NUnit.Framework;
using System;
using TopDownProteomics.Chemistry;
using TopDownProteomics.Chemistry.Unimod;

namespace TopDownProteomics.Tests
{
    [TestFixture]
    public class UnimodCompositionTests
    {
        IUnimodCompositionAtomProvider atomProvider = new MockAtomProvider();

        [Test]
        public void InvalidArgumentsStrings()
        {
            Assert.Throws<ArgumentNullException>(() => UnimodComposition.CreateFromFormula(null, atomProvider));
            Assert.Throws<ArgumentNullException>(() => UnimodComposition.CreateFromFormula(string.Empty, atomProvider));
        }

        [Test]
        public void SingleAtomTest()
        {
            var composition = UnimodComposition.CreateFromFormula("C", atomProvider);
            var cardinalities = composition.GetAtomCardinalities();

            Assert.AreEqual(1, cardinalities.Count);
            Assert.AreEqual(atomProvider.GetUnimodCompositionAtom("C"), cardinalities[0].Atom);
            Assert.AreEqual(1, cardinalities[0].Count);

            composition = UnimodComposition.CreateFromFormula("C(1)", atomProvider);
            cardinalities = composition.GetAtomCardinalities();

            Assert.AreEqual(1, cardinalities.Count);
            Assert.AreEqual(atomProvider.GetUnimodCompositionAtom("C"), cardinalities[0].Atom);
            Assert.AreEqual(1, cardinalities[0].Count);

            composition = UnimodComposition.CreateFromFormula("C(100)", atomProvider);
            cardinalities = composition.GetAtomCardinalities();

            Assert.AreEqual(1, cardinalities.Count);
            Assert.AreEqual(atomProvider.GetUnimodCompositionAtom("C"), cardinalities[0].Atom);
            Assert.AreEqual(100, cardinalities[0].Count);
        }

        [Test]
        public void WaterTest()
        {
            const string formula = "H(2) O";

            var composition = UnimodComposition.CreateFromFormula(formula, atomProvider);

            var cardinalities = composition.GetAtomCardinalities();

            Assert.AreEqual(2, cardinalities.Count);
            Assert.AreEqual(atomProvider.GetUnimodCompositionAtom("H"), cardinalities[0].Atom);
            Assert.AreEqual(2, cardinalities[0].Count);
            Assert.AreEqual(atomProvider.GetUnimodCompositionAtom("O"), cardinalities[1].Atom);
            Assert.AreEqual(1, cardinalities[1].Count);

            var chemicalFormula = composition.GetChemicalFormula();
            Assert.AreEqual(1.007825035 * 2 + 15.99491463, chemicalFormula.GetMass(MassType.Monoisotopic), 0.00001);
            Assert.AreEqual(1.00794 * 2 + 15.9994, chemicalFormula.GetMass(MassType.Average), 0.00001);
        }

        [Test]
        public void NegativeTest()
        {
            const string formula = "C(-2) O(3)";

            var composition = UnimodComposition.CreateFromFormula(formula, atomProvider);

            var cardinalities = composition.GetAtomCardinalities();

            Assert.AreEqual(2, cardinalities.Count);
            Assert.AreEqual(atomProvider.GetUnimodCompositionAtom("C"), cardinalities[0].Atom);
            Assert.AreEqual(-2, cardinalities[0].Count);
            Assert.AreEqual(atomProvider.GetUnimodCompositionAtom("O"), cardinalities[1].Atom);
            Assert.AreEqual(3, cardinalities[1].Count);
        }

        [Test]
        public void FucoseTest()
        {
            const string formula = "dHex";

            var composition = UnimodComposition.CreateFromFormula(formula, atomProvider);

            var cardinalities = composition.GetAtomCardinalities();

            Assert.AreEqual(1, cardinalities.Count);
            Assert.AreEqual(atomProvider.GetUnimodCompositionAtom("dHex"), cardinalities[0].Atom);
            Assert.AreEqual(1, cardinalities[0].Count);

            var chemicalFormula = composition.GetChemicalFormula();
            Assert.AreEqual(146.057909, chemicalFormula.GetMass(MassType.Monoisotopic), 0.00001);
            Assert.AreEqual(146.1412, chemicalFormula.GetMass(MassType.Average), 0.001);
        }

        private class MockAtomProvider : IUnimodCompositionAtomProvider
        {
            private UnimodCompositionAtom[] _atoms;

            public MockAtomProvider()
            {
                var elementProvider = new MockElementProvider();

                _atoms = new UnimodCompositionAtom[128];
                _atoms['H'] = new UnimodCompositionAtom("H", "Hydrogen", new[] {
                    new EntityCardinality<IElement>(elementProvider.GetElement(1), 1) });
                _atoms['C'] = new UnimodCompositionAtom("C", "Carbon", new[] {
                    new EntityCardinality<IElement>(elementProvider.GetElement(6), 1) });
                _atoms['O'] = new UnimodCompositionAtom("O", "Oxygen", new[] {
                    new EntityCardinality<IElement>(elementProvider.GetElement(8), 1) });

                // dHex(Fucose) C6H12O5 - H20 = C6H10O4
                _atoms['d'] = new UnimodCompositionAtom("dHex", "Deoxy-hexose", new[] 
                {
                    new EntityCardinality<IElement>(elementProvider.GetElement(6), 6),
                    new EntityCardinality<IElement>(elementProvider.GetElement(1), 10),
                    new EntityCardinality<IElement>(elementProvider.GetElement(8), 4),
                });
            }

            public UnimodCompositionAtom GetUnimodCompositionAtom(string symbol)
            {
                return _atoms[symbol[0]];
            }
        }
    }
}