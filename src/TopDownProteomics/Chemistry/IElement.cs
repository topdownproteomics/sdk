using System.Collections.Generic;

namespace TopDownProteomics.Chemistry
{
    /// <summary>
    /// Represents a single chemical element. Elements are usually comprised of multiple
    /// isotopes, with the element mass being a weighted average of all the
    /// isotopes atomic masses weighted by their natural relative abundance.
    /// </summary>
    public interface IElement : IHasMass
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
        IReadOnlyCollection<IIsotope> Isotopes { get; }
    }
}