﻿using System.Collections.Generic;
using System.Linq;

namespace TopDownProteomics.Chemistry
{
    /// <summary>A class containing chemistry utility functions.</summary>
    public static class ChemistryUtility
    {
        /// <summary>Gets the chemical formula as a string in Hill notation.</summary>
        /// <param name="chemicalFormula">The chemical formula.</param>
        /// <returns></returns>
        public static string GetChemicalFormulaString(this IChemicalFormula chemicalFormula)
        {
            // Local function for converting a single element to a string.
            string GetElementString(IEntityCardinality<IElement> element)
            {
                if (element.Count == 0) // Don't write zero.
                    return string.Empty;

                if (element.Count == 1)
                    return element.Entity.Symbol;

                // In all other cases need to write out count too.
                return $"{element.Entity.Symbol}{element.Count}";
            }

            // Main function.
            ICollection<IEntityCardinality<IElement>> elements = chemicalFormula.GetElements().ToList();
            IList<string> elementStrings = new List<string>();

            // Look for carbon first.  If it exists, write it and then hydrogen.
            IEntityCardinality<IElement> carbon = elements.SingleOrDefault(e => e.Entity.AtomicNumber == 6);
            if (carbon is not null && carbon.Count != 0)
            {
                elementStrings.Add(GetElementString(carbon));
                elements.Remove(carbon);

                IEntityCardinality<IElement> hydrogen = elements.SingleOrDefault(e => e.Entity.AtomicNumber == 1);
                if (hydrogen is not null && hydrogen.Count != 0)
                {
                    elementStrings.Add(GetElementString(hydrogen));
                    elements.Remove(hydrogen);
                }
            }

            // Write out the rest in alphabetical order.
            foreach (IEntityCardinality<IElement> element in elements.OrderBy(e => e.Entity.Symbol))
            {
                if (element.Count != 0)
                {
                    elementStrings.Add(GetElementString(element));
                }
            }

            return string.Join("", elementStrings);
        }
    }
}