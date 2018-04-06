using System.Collections.Generic;
using System.Linq;

namespace TopDownProteomics.Chemistry
{
    /// <summary>
    /// Default implementation of an element.
    /// </summary>
    /// <seealso cref="IElement" />
    public class Element : IElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Element"/> class.
        /// </summary>
        /// <param name="atomicNumber">The atomic number.</param>
        /// <param name="symbol">The symbol.</param>
        /// <param name="isotopes">The isotopes.</param>
        public Element(int atomicNumber, string symbol, IReadOnlyCollection<IIsotope> isotopes)
        {
            this.AtomicNumber = atomicNumber;
            this.Symbol = symbol;
            this.Isotopes = isotopes;
        }

        /// <summary>
        /// Get the element's atomic number
        /// </summary>
        public int AtomicNumber { get; }

        /// <summary>
        /// Gets the element's symbol
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// Gets the Isotopes that make up the element
        /// </summary>
        public IReadOnlyCollection<IIsotope> Isotopes { get; }

        /// <summary>
        /// Gets the mass.
        /// </summary>
        /// <param name="massType">Type of the mass.</param>
        /// <returns></returns>
        public double GetMass(MassType massType)
        {
            return massType == MassType.Monoisotopic
                ? this.Isotopes.FirstWithMax(isotope => isotope.RelativeAbundance).AtomicMass // Should be most abundant, naturally occuring isotope
                : this.Isotopes.Sum(isotope => isotope.AtomicMass * isotope.RelativeAbundance);
        }
    }
}