using System;
using System.Collections.Generic;

namespace TopDownProteomics.Chemistry
{
    /// <summary>
    /// A collection of different elements in various quantities.
    /// </summary>
    public interface IChemicalFormula : IHasMass, IEquatable<IChemicalFormula>
    {
        /// <summary>
        /// Gets the elements.
        /// </summary>
        /// <returns></returns>
        IReadOnlyCollection<IEntityCardinality<IElement>> GetElements();

        ///// <summary>
        ///// Gets the independent isotopes.
        ///// </summary>
        ///// <returns></returns>
        //IEntityCardinality<IIsotope> GetIndependentIsotopes();

        /// <summary>
        /// Adds the specified formula.
        /// </summary>
        /// <param name="formula">The formula.</param>
        /// <returns></returns>
        IChemicalFormula Add(IChemicalFormula formula);

        /// <summary>
        /// Multiplies the formula by the specified multiplier.
        /// </summary>
        /// <param name="multiplier">The multiplier.</param>
        /// <returns></returns>
        IChemicalFormula Multiply(int multiplier);
    }
}