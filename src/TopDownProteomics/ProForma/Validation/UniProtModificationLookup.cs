using System;
using System.Collections.Generic;
using TopDownProteomics.Chemistry;
using TopDownProteomics.IO.UniProt;

namespace TopDownProteomics.ProForma.Validation
{
    /// <summary>
    /// Lookup for UniProt modifications.
    /// </summary>
    public class UniProtModificationLookup : ModificationLookupBase<UniprotModification>
    {
        private readonly IElementProvider _elementProvider;

        private UniProtModificationLookup(IElementProvider elementProvider)
        {
            _elementProvider = elementProvider;
        }

        /// <summary>
        /// The ProForma key.
        /// </summary>
        protected override ProFormaEvidenceType Key => ProFormaEvidenceType.UniProt;

        /// <summary>
        /// Initializes the <see cref="ResidModificationLookup" /> class.
        /// </summary>
        /// <param name="modifications">The modifications.</param>
        /// <param name="elementProvider">The element provider.</param>
        /// <returns></returns>
        public static IProteoformModificationLookup CreateFromModifications(IEnumerable<UniprotModification> modifications,
            IElementProvider elementProvider)
        {
            var lookup = new UniProtModificationLookup(elementProvider);

            lookup.SetupModificationArray(modifications);

            return lookup;
        }

        /// <summary>
        /// Gets the chemical formula.
        /// </summary>
        /// <param name="modification">The modification.</param>
        /// <returns></returns>
        protected override IChemicalFormula? GetChemicalFormula(UniprotModification modification)
        {
            string? formula = modification.CorrectionFormula;

            if (string.IsNullOrEmpty(formula))
                return null;

            string[] cells = formula.Split(' ');

            var elements = new List<IEntityCardinality<IElement>>();

            for (int i = 0; i < cells.Length; i++)
            {
                // Find last index for element name
                int j = cells[i].Length - 1;
                while (char.IsDigit(cells[i][j]) || cells[i][j] == '-')
                {
                    j--;
                }

                string elementSymbol = cells[i].Substring(0, j + 1);
                int count = Convert.ToInt32(cells[i].Substring(j + 1));

                if (count != 0)
                    elements.Add(new EntityCardinality<IElement>(_elementProvider.GetElement(elementSymbol), count));
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
            if (value.StartsWith("PTM-"))
                return value.Substring(4);

            return value;
        }
    }
}