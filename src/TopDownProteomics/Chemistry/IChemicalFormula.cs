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

    /// <summary>
    /// Extensions for the chemical formula.
    /// </summary>
    public static class ChemicalFormulaExtensions
    {
        /// <summary>
        /// Creates a text based representation of a chemical formula.
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        [Obsolete("Duplicated, please use extension method ChemistryUtility.GetChemicalFormulaString().")]
        public static string GetTextFormat(this IChemicalFormula formula)
        {
            return formula.GetChemicalFormulaString();
        }
    }
}