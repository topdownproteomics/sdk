using System.Collections.Generic;
using TopDownProteomics.Chemistry;
using TopDownProteomics.IO;
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
        protected override ProFormaEvidenceType EvidenceType => ProFormaEvidenceType.UniProt;

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
            if (value.StartsWith("PTM-"))
                return value.Substring(4);

            return value;
        }
    }
}