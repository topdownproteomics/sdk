using System;
using System.Collections.Generic;
using TopDownProteomics.Chemistry;
using TopDownProteomics.IO.PsiMod;

namespace TopDownProteomics.ProForma.Validation
{
    /// <summary>
    /// Lookup for PSI-MOD modifications.
    /// </summary>
    public class PsiModModificationLookup : ModificationLookupBase<PsiModTerm>
    {
        private readonly IElementProvider _elementProvider;

        private PsiModModificationLookup(IElementProvider elementProvider)
        {
            _elementProvider = elementProvider;
        }

        /// <summary>
        /// The ProForma key.
        /// </summary>
        protected override ProFormaEvidenceType Key => ProFormaEvidenceType.PsiMod;

        /// <summary>
        /// Initializes the <see cref="ResidModificationLookup" /> class.
        /// </summary>
        /// <param name="modifications">The modifications.</param>
        /// <param name="elementProvider">The element provider.</param>
        /// <returns></returns>
        public static IProteoformModificationLookup CreateFromModifications(IEnumerable<PsiModTerm> modifications,
            IElementProvider elementProvider)
        {
            var lookup = new PsiModModificationLookup(elementProvider);

            lookup.SetupModificationArray(modifications);

            return lookup;
        }

        /// <summary>
        /// Gets the chemical formula.
        /// </summary>
        /// <param name="modification">The modification.</param>
        /// <returns></returns>
        protected override IChemicalFormula? GetChemicalFormula(PsiModTerm modification)
        {
            string? formula = modification.DiffFormula;

            if (string.IsNullOrEmpty(formula) || formula == "none")
                return null;

            string[] cells = formula.Split(' ');

            var elements = new List<IEntityCardinality<IElement>>();

            for (int i = 0; i < cells.Length; i += 2)
            {
                if (cells[i] == "+")
                    continue;

                int count = Convert.ToInt32(cells[i + 1]);

                if (count != 0)
                {
                    // Handle formal charge by adding or removing hydrogen atoms
                    if (modification.FormalCharge != 0 && cells[i] == "H")
                        count -= modification.FormalCharge;

                    if (cells[i][0] == '(') // Fixed isotope.
                    {
                        int endIsotopeIndex = cells[i].IndexOf(')');
                        string isotopeStr = cells[i].Substring(1, endIsotopeIndex - 1);
                        int fixedIsotope = Convert.ToInt32(isotopeStr);
                        elements.Add(new EntityCardinality<IElement>(_elementProvider.GetElement(cells[i].Substring(endIsotopeIndex + 1), fixedIsotope), count));
                    }
                    else
                    {
                        elements.Add(new EntityCardinality<IElement>(_elementProvider.GetElement(cells[i]), count));
                    }
                }
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
            if (value.StartsWith("MOD:"))
                return value.Substring(4);

            return value;
        }
    }
}