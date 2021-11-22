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
        /// Gets a value indicating whether this instance is default modification type.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is default modification type; otherwise, <c>false</c>.
        /// </value>
        protected override bool IsDefaultModificationType => true;

        /// <summary>
        /// The ProForma key.
        /// </summary>
        protected override ProFormaEvidenceType EvidenceType => ProFormaEvidenceType.PsiMod;

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
            return modification.GetChemicalFormula(_elementProvider);
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