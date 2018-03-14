using System.Collections.Generic;

namespace TestLibNamespace.Northwestern
{
    /// <summary>
    /// A collection of different elements in various quantities.
    /// </summary>
    public interface IChemicalFormula : IDualMass
    {
        /// <summary>
        /// Gets the quantity for the specified element.
        /// </summary>
        int this[IElement element] { get; }

        /// <summary>
        /// Gets the element counts.
        /// </summary>
        IEnumerable<IElementCount> ElementCounts { get; }

        /// <summary>
        /// Determines if this formula equals another.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        bool Equals(IChemicalFormula other);

        /// <summary>
        /// Determines whether [contains] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified element]; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(IElement element);

        /// <summary>
        /// Adds the specified formula.
        /// </summary>
        /// <param name="formula">The formula.</param>
        /// <returns></returns>
        IChemicalFormula Add(IChemicalFormula formula);

        /// <summary>
        /// Subtracts the specified formula.
        /// </summary>
        /// <param name="formula">The formula.</param>
        /// <returns></returns>
        IChemicalFormula Subtract(IChemicalFormula formula);

        /// <summary>
        /// Multiplies the formula by the specified multiplier.
        /// </summary>
        /// <param name="multiplier">The multiplier.</param>
        /// <returns></returns>
        IChemicalFormula Multiply(int multiplier);
    }
}