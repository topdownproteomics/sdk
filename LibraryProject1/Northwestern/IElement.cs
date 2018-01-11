using System.Collections.Generic;

namespace TestLibNamespace.Northwestern
{
    /// <summary>
    /// An Element is a wrapper around a collection of Isotopes
    /// </summary>
    public interface IElement : IDualMass, IHasName
    {
        /// <summary>
        /// Get the element's atomic number
        /// </summary>
        int AtomicNumber { get; }

        /// <summary>
        /// Gets the element's symbol
        /// </summary>
        string Symbol { get; }

        /// <summary>
        /// Gets the Isotopes that make up the element
        /// </summary>
        IList<IIsotope> Isotopes { get; }
    }
}