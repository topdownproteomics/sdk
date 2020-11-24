using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TopDownProteomics.Chemistry;

namespace TopDownProteomics.Tests.Chemistry
{
    [TestFixture]
    public class ChemicalFormulaTest
    {
        IElementProvider _elementProvider;

        [OneTimeSetUp]
        public void Setup()
        {
            _elementProvider = new MockElementProvider();
        }

        [Test]
        public void WaterParseTest()
        {
            string formulaString = "H2O";

            var chemicalFormula = this.SimpleParseTest(formulaString, new[]
            {
                Tuple.Create("H", 2),
                Tuple.Create("O", 1)
            });

            Assert.AreEqual(ChemicalFormula.Water(_elementProvider), chemicalFormula);
        }

        [Test]
        public void MulticharacterElementParseTest()
        {
            string formulaString = "He2O";

            var chemicalFormula = this.SimpleParseTest(formulaString, new[]
            {
                Tuple.Create("He", 2),
                Tuple.Create("O", 1)
            });

            Assert.AreNotEqual(ChemicalFormula.Water(_elementProvider), chemicalFormula);
        }

        [Test]
        public void ComplexParseTest()
        {
            string formulaString = "C12H20O2";

            this.SimpleParseTest(formulaString, new[]
            {
                Tuple.Create("C", 12),
                Tuple.Create("H", 20),
                Tuple.Create("O", 2)
            });
        }

        [Test]
        public void DuplicateElementTest()
        {
            string formulaString = "CHCHO";

            this.SimpleParseTest(formulaString, new[]
            {
                Tuple.Create("C", 2),
                Tuple.Create("H", 2),
                Tuple.Create("O", 1)
            });
        }

        [Test]
        public void NegativeParseTest()
        {
            string formulaString = "HN-1O2";

            this.SimpleParseTest(formulaString, new[]
            {
                Tuple.Create("H", 1),
                Tuple.Create("N", -1),
                Tuple.Create("O", 2)
            });
        }

        [Test]
        public void IncorrectNegativeParseTest()
        {
            // No longer accepting Unimod format
            string formulaString = "C6H12N-O";
            Assert.IsFalse(ChemicalFormula.TryParseString(formulaString, _elementProvider, out _));
        }

        [Test]
        public void IsotopeParseTest()
        {
            string formulaString = "[13C2][12C-2]H2N";

            this.SimpleParseTest(formulaString, new[]
            {
                Tuple.Create("13C", 2),
                Tuple.Create("12C", -2),
                Tuple.Create("H", 2),
                Tuple.Create("N", 1)
            });
        }

        [Test]
        public void IsotopeParseTest2()
        {
            string formulaString = "[13C2]";

            this.SimpleParseTest(formulaString, new[]
            {
                Tuple.Create("13C", 2)
            });
        }

        [Test]
        public void IsotopeParseTest3()
        {
            string formulaString = "C2[12C-2]H2N";

            this.SimpleParseTest(formulaString, new[]
            {
                Tuple.Create("C", 2),
                Tuple.Create("12C", -2),
                Tuple.Create("H", 2),
                Tuple.Create("N", 1)
            });
        }

        [Test]
        public void MergeDuplicatesTest()
        {
            string formulaString = "CCHHHCC";

            this.SimpleParseTest(formulaString, new[]
            {
                Tuple.Create("C", 4),
                Tuple.Create("H", 3)
            });
        }

        // RTF 2020: This was pulled because it wasn't really needed for ProForma v2
        //[Test]
        //public void CondensedTest()
        //{
        //    string formulaString = "CH3(CH2)4CH3";

        //    this.SimpleParseTest(formulaString, new[]
        //    {
        //        Tuple.Create("C", 6),
        //        Tuple.Create("H", 14)
        //    });
        //}

        [Test]
        public void IncorrectFormatTest()
        {
            // No longer accepting Unimod format
            string formulaString = "H(2) O";
            Assert.IsFalse(ChemicalFormula.TryParseString(formulaString, _elementProvider, out _));
        }

        [Test]
        public void WaterTest()
        {
            IElementProvider provider = new MockElementProvider();
            IElement h = provider.GetElement("H");
            IElement o = provider.GetElement("O");

            var formula = ChemicalFormula.Water(provider);
            var elements = formula.GetElements();

            Assert.AreEqual(2, elements.Count);
            Assert.AreEqual(2, elements.Single(x => x.Entity.Symbol == "H").Count);
            Assert.AreEqual(1, elements.Single(x => x.Entity.Symbol == "O").Count);

            Assert.AreEqual(h.Isotopes.FirstWithMax(x => x.RelativeAbundance).AtomicMass * 2
                + o.Isotopes.FirstWithMax(x => x.RelativeAbundance).AtomicMass,
                formula.GetMass(MassType.Monoisotopic));
            Assert.AreEqual(h.Isotopes.Sum(x => x.AtomicMass * x.RelativeAbundance) * 2
                + o.Isotopes.Sum(x => x.AtomicMass * x.RelativeAbundance),
                formula.GetMass(MassType.Average));
        }

        [Test]
        public void MassTest()
        {
            // Using simple 100% abundances
            var provider = new MockElementProvider();
            provider.OverwriteElement(new Element(1, "H", new[]
            {
                new Isotope(1, 0, 1.0)
            }));
            provider.OverwriteElement(new Element(8, "O", new[]
            {
                new Isotope(16, 8, 1.0)
            }));

            var formula = ChemicalFormula.Water(provider);
            var elements = formula.GetElements();

            Assert.AreEqual(18, formula.GetMass(MassType.Monoisotopic));
            Assert.AreEqual(18, formula.GetMass(MassType.Average));

            // Switch to 75/25
            provider.OverwriteElement(new Element(8, "O", new[]
            {
                new Isotope(16, 8, 0.75),
                new Isotope(17, 9, 0.25)
            }));

            formula = ChemicalFormula.Water(provider);
            elements = formula.GetElements();

            Assert.AreEqual(18, formula.GetMass(MassType.Monoisotopic));
            Assert.AreEqual(18.25, formula.GetMass(MassType.Average));
        }

        [Test]
        public void ChemicalFormulaCompareTest()
        {
            var provider = new MockElementProvider();
            var formula = ChemicalFormula.Water(provider);

            var formula2 = ChemicalFormula.Water(provider);

            IElement h = provider.GetElement("H");
            IElement o = provider.GetElement("O");
            var formula3 = new ChemicalFormula(new[]
            {
                new EntityCardinality<IElement>(h, 1),
                new EntityCardinality<IElement>(o, 2),
            });

            Assert.AreEqual(formula, formula2);
            Assert.AreNotEqual(formula, formula3);

            // Equality when element object is different.
            IElement anotherH = new Element(1, "H", h.Isotopes);
            IElement anotherO = new Element(1, "O", o.Isotopes);
            ChemicalFormula formula4 = new ChemicalFormula(new[]
            {
                new EntityCardinality<IElement>(anotherH, 2),
                new EntityCardinality<IElement>(anotherO, 1),
            });

            Assert.IsTrue(formula.Equals(formula4));
            Assert.IsTrue(formula2.Equals(formula4));
            Assert.IsFalse(formula3.Equals(formula4));
        }

        [Test]
        public void Carbon13Test()
        {
            IElementProvider provider = new MockElementProvider();
            IElement c = provider.GetElement("C");
            IElement c13 = provider.GetElement("C", 13);

            var formula = new ChemicalFormula(new[]
            {
                new EntityCardinality<IElement>(c, 1),
                new EntityCardinality<IElement>(c13, 1),
            });

            var elements = formula.GetElements();

            Assert.AreEqual(2, elements.Count);

            // Check masses
            Assert.AreEqual(c.Isotopes.FirstWithMax(x => x.RelativeAbundance).AtomicMass
                + c13.Isotopes.FirstWithMax(x => x.RelativeAbundance).AtomicMass,
                formula.GetMass(MassType.Monoisotopic));
            Assert.AreEqual(c.Isotopes.Sum(x => x.AtomicMass * x.RelativeAbundance)
                + c13.Isotopes.Sum(x => x.AtomicMass * x.RelativeAbundance),
                formula.GetMass(MassType.Average));
            Assert.AreEqual(c13.GetMass(MassType.Monoisotopic), c13.GetMass(MassType.Average));
        }

        [Test]
        public void IsotopesTest()
        {
            var provider = new MockElementProvider();
            IElement h = provider.GetElement(1);
            IElement h1 = provider.GetElement(1, 1);
            IElement h2 = provider.GetElement(1, 2);

            // These two are equal.
            ChemicalFormula formula0 = new ChemicalFormula(new[]
            {
                new EntityCardinality<IElement>(h, 2)
            });
            ChemicalFormula formula1 = new ChemicalFormula(new[]
            {
                new EntityCardinality<IElement>(h, 2)
            });

            ChemicalFormula formula2 = new ChemicalFormula(new[]
            {
                new EntityCardinality<IElement>(h1, 2)
            });

            // These two are equal.
            ChemicalFormula formula3 = new ChemicalFormula(new[]
            {
                new EntityCardinality<IElement>(h, 1),
                new EntityCardinality<IElement>(h1, 1)
            });
            ChemicalFormula formula4 = new ChemicalFormula(new[]
            {
                new EntityCardinality<IElement>(h1, 1),
                new EntityCardinality<IElement>(h, 1)
            });

            ChemicalFormula formula5 = new ChemicalFormula(new[]
            {
                new EntityCardinality<IElement>(h1, 1),
                new EntityCardinality<IElement>(h2, 1)
            });

            ChemicalFormula[] formulas = new[]
            {
                formula0, formula1, formula2, formula3, formula4, formula5,
            };

            for (int i = 0; i < 6; i++)
            {
                for (int j = i; j < 6; j++)
                {
                    ChemicalFormula fi = formulas[i];
                    ChemicalFormula fj = formulas[j];

                    if (
                            i == j ||
                            (i == 0 && j == 1) ||
                            (i == 3 && j == 4)
                        )
                    {
                        Assert.IsTrue(fi.Equals(fj));
                    }
                    else
                    {
                        Assert.IsFalse(fi.Equals(fj));
                    }
                }
            }
        }

        [Test]
        public void IgnoreZerosTest()
        {
            string formulaString = "C2H2N0O1";

            this.SimpleParseTest(formulaString, new[]
            {
                Tuple.Create("C", 2),
                Tuple.Create("H", 2),
                Tuple.Create("O", 1),
            });
        }

        private IChemicalFormula SimpleParseTest(string formulaString, params Tuple<string, int>[] elements)
        {
            bool success = ChemicalFormula.TryParseString(formulaString, _elementProvider, out IChemicalFormula chemicalFormula);

            Assert.IsTrue(success);
            Assert.IsNotNull(chemicalFormula);

            IReadOnlyCollection<IEntityCardinality<IElement>> elementCollection = chemicalFormula.GetElements();
            Assert.AreEqual(elements.Length, elementCollection.Count, "Element Count");

            foreach (var (symbol, count) in elements)
            {
                IEntityCardinality<IElement> h = elementCollection.SingleOrDefault(e => e.Entity.Symbol == symbol);
                Assert.IsNotNull(h, $"For element '{symbol}{count}'");
                Assert.AreEqual(count, h.Count, $"For element '{symbol}{count}'");
            }

            return chemicalFormula;
        }
    }
}