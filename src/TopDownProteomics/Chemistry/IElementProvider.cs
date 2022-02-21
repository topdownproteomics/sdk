using System;
using System.Collections.Generic;

namespace TopDownProteomics.Chemistry
{
    /// <summary>
    /// Provides elements.
    /// </summary>
    public interface IElementProvider
    {
        /// <summary>
        /// Gets the element by atomic number.
        /// </summary>
        /// <param name="atomicNumber">The atomic number.</param>
        /// <param name="fixedIsotopeNumber">Get a fixed isotope element with the given number of subatomic particles in the nucleus.</param>
        /// <returns></returns>
        IElement GetElement(int atomicNumber, int? fixedIsotopeNumber = null);

        /// <summary>
        /// Gets the element by symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="fixedIsotopeNumber">Get a fixed isotope element with the given number of subatomic particles in the nucleus.</param>
        /// <returns></returns>
        IElement GetElement(ReadOnlySpan<char> symbol, int? fixedIsotopeNumber = null);

        /// <summary>
        /// Gets all of the elements.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IElement> GetElements();
    }
}