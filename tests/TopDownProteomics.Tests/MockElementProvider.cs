using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TopDownProteomics.Chemistry;

namespace TopDownProteomics.Tests
{
    public class MockElementProvider : IElementProvider
    {
        private IElement[] _elements;
        private IElement _carbon12;
        private IElement _carbon13;

        public MockElementProvider()
        {
            _elements = new IElement[128];
            _elements[1] = new Element(1, "H", new[]
            {
                new Isotope(1.007825032, 0, 0.999885),
                new Isotope(2.014101778, 1, 0.000115)
            });
            _elements[2] = new Element(2, "He", new[]
            {
                new Isotope(3.0160293191, 1, 1.34e-06),
                new Isotope(4.00260325415, 2, 0.99999866)
            });
            _elements[5] = new Element(5, "B", new[]
            {
                new Isotope(10.01293695, 5, 0.199),
                new Isotope(11.00930536, 6, 0.801)
            });
            _elements[6] = new Element(6, "C", new[]
            {
                new Isotope(12.0, 6, 0.9893),
                new Isotope(13.0033548378, 7, 0.0107)
            });
            _elements[7] = new Element(7, "N", new[]
            {
                new Isotope(14.0030740052, 7, 0.99632),
                new Isotope(15.0001088984, 8, 0.00368)
            });
            _elements[8] = new Element(8, "O", new[]
            {
                new Isotope(15.99491463, 8, 0.99757),
                new Isotope(16.9991312, 9, 0.00038),
                new Isotope(17.9991603, 10, 0.00205)
            });
            _elements[15] = new Element(15, "P", new[]
            {
                new Isotope(30.97376163, 16, 1.0000)
            });
            _elements[16] = new Element(16, "S", new[]
            {
                new Isotope(31.97207100, 16, 0.9499),
                new Isotope(32.97145876, 17, 0.0075),
                new Isotope(33.96786690, 18, 0.0425),
                new Isotope(35.96708076, 20, 0.0001)
            });
            _elements[34] = new Element(34, "Se", new[]
            {
                new Isotope(73.9224764, 40, 0.0089),
                new Isotope(75.9192136, 42, 0.0937),
                new Isotope(76.9199140, 43, 0.0763),
                new Isotope(77.9173091, 44, 0.2377),
                new Isotope(79.9165213, 46, 0.4961),
                new Isotope(81.9166994, 48, 0.0873)
            });

            _carbon12 = new Element(6, "12C", new[]
            {
                new Isotope(12.0, 6, 1.0),
            });

            _carbon13 = new Element(6, "13C", new[]
            {
                new Isotope(13.0033548378, 7, 1.0)
            });
        }

        public void OverwriteElement(IElement element)
        {
            _elements[element.AtomicNumber] = element;
        }

        public IElement GetElement(int atomicNumber, int? fixedIsotopeNumber = null)
        {
            if (atomicNumber == 6 && fixedIsotopeNumber == 12) return _carbon12;
            if (atomicNumber == 6 && fixedIsotopeNumber == 13) return _carbon13;

            IElement element = _elements[atomicNumber];
            return this.GetFixedIsotopeElement(element, fixedIsotopeNumber);
        }

        public IElement GetElement(ReadOnlySpan<char> symbol, int? fixedIsotopeNumber = null)
        {
            if (symbol[0] == 'C' && fixedIsotopeNumber == 12) return _carbon12;
            if (symbol[0] == 'C' && fixedIsotopeNumber == 13) return _carbon13;

            var symbolString = symbol.ToString();
            IElement element = _elements.Single(x => x?.Symbol == symbolString);
            return this.GetFixedIsotopeElement(element, fixedIsotopeNumber);
        }

        private IElement GetFixedIsotopeElement(IElement element, int? fixedIsotopeNumber)
        {
            if (fixedIsotopeNumber == null)
            {
                return element;
            }

            IIsotope oldIsotope = element.Isotopes
                .Single(x => x.NeutronCount == fixedIsotopeNumber - element.AtomicNumber);
            IIsotope newIsotope = new Isotope(element.AtomicNumber, oldIsotope.NeutronCount, 1.0);

            return new Element(element.AtomicNumber, element.Symbol,
                new ReadOnlyCollection<IIsotope>(new[] { newIsotope }));
        }

        public IEnumerable<IElement> GetElements()
        {
            return _elements.Where(x => x != null);
        }

        public IElement GetElement(string symbol, int? fixedIsotopeNumber = null)
        {
            return this.GetElement(symbol.AsSpan(), fixedIsotopeNumber);
        }
    }
}