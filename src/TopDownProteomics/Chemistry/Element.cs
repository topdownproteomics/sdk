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

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(IElement other)
        {
            // Check for null.
            if (other == null)
            {
                return false;
            }

            // Check for object equality.
            if (this == other)
            {
                return true;
            }

            IReadOnlyCollection<IIsotope> otherIsotopes = other.Isotopes;

            // Check the number of isotopes.
            if (this.Isotopes.Count != otherIsotopes.Count)
            {
                return false;
            }
            
            // Check all isotopes.
            foreach (IIsotope isotope in this.Isotopes)
            {
                IIsotope otherIsotope = otherIsotopes.SingleOrDefault(i => i.NeutronCount == isotope.NeutronCount);

                // Make sure the other element has this isotope.
                if (otherIsotope == null)
                {
                    return false;
                }

                // Check abundances.
                if (isotope.RelativeAbundance != otherIsotope.RelativeAbundance)
                {
                    return false;
                }
            }

            return true;
        }
    }
}