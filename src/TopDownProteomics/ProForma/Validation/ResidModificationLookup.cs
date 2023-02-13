using System.Collections.Generic;
using TopDownProteomics.Chemistry;
using TopDownProteomics.IO.Resid;

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
        protected override ProFormaEvidenceType EvidenceType => ProFormaEvidenceType.Resid;

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
        protected override ChemicalFormula? GetChemicalFormula(ResidModification modification)
        {
            return modification.GetChemicalFormula(_elementProvider);
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