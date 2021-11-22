using System;
using System.Collections.Generic;
using System.Text;

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
        public static string GetTextFormat(this IChemicalFormula formula)
        {
            var chemicalFormula = new StringBuilder();

            foreach (var element in formula.GetElements())
            {
                if (element.Count != 0)
                {
                    chemicalFormula.Append(element.Entity.Symbol);

                    if (element.Count != 1)
                    {
                        chemicalFormula.Append(element.Count.ToString());
                    }
                }
            }

            return chemicalFormula.ToString();
        }
    }
}