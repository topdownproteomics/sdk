using System;
using System.Collections.Generic;
using System.Linq;
using TopDownProteomics.Chemistry;
using TopDownProteomics.IO.Resid;
using TopDownProteomics.Proteomics;

namespace TopDownProteomics.ProForma.Validation
{
    /// <summary>
    /// Lookup for RESID modifications.
    /// </summary>
    public class ResidModificationLookup : ModificationLookupBase<ResidModification>
    {
        private readonly IElementProvider _elementProvider;

        private ResidModificationLookup(IElementProvider elementProvider)
        {
            _elementProvider = elementProvider;
        }

        /// <summary>
        /// The ProForma key.
        /// </summary>
        protected override string Key => ProFormaKey.Resid;

        /// <summary>
        /// Initializes the <see cref="ResidModificationLookup" /> class.
        /// </summary>
        /// <param name="modifications">The modifications.</param>
        /// <param name="elementProvider">The element provider.</param>
        /// <returns></returns>
        public static IProteoformModificationLookup CreateFromModifications(IEnumerable<ResidModification> modifications,
            IElementProvider elementProvider)
        {
            var lookup = new ResidModificationLookup(elementProvider);

            lookup.SetupModificationArray(modifications);

            return lookup;
        }

        /// <summary>
        /// Gets the chemical formula.
        /// </summary>
        /// <param name="modification">The modification.</param>
        /// <returns></returns>
        protected override IChemicalFormula GetChemicalFormula(ResidModification modification)
        {
            string formula = modification.DiffFormula;

            if (string.IsNullOrEmpty(formula))
                return null;

            string[] cells = formula.Split(' ');

            var elements = new List<IEntityCardinality<IElement>>();

            for (int i = 0; i < cells.Length; i += 2)
            {
                if (cells[i] == "+")
                    continue;

                int count = Convert.ToInt32(cells[i + 1]);

                if (count != 0)
                    elements.Add(new EntityCardinality<IElement>(_elementProvider.GetElement(cells[i]), count));
            }

            return new ChemicalFormula(elements);
        }

        /// <summary>
        /// Removes the prefix.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected override string RemovePrefix(string value)
        {
            // Remove prefix AA
            if (value.StartsWith("AA"))
                return value.Substring(2);

            return value;
        }
    }
}