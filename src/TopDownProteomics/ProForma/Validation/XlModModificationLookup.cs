using System.Collections.Generic;
using TopDownProteomics.Chemistry;
using TopDownProteomics.IO.Xlmod;

namespace TopDownProteomics.ProForma.Validation
{
    /// <summary>
    /// Lookup for XlMod modifications.
    /// </summary>
    public class XlModModificationLookup : ModificationLookupBase<XlmodTerm>
    {
        private readonly IElementProvider _elementProvider;

        private XlModModificationLookup(IElementProvider elementProvider)
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
        protected override ProFormaEvidenceType EvidenceType => ProFormaEvidenceType.XlMod;

        /// <summary>
        /// Initializes the <see cref="ResidModificationLookup" /> class.
        /// </summary>
        /// <param name="modifications">The modifications.</param>
        /// <param name="elementProvider">The element provider.</param>
        /// <returns></returns>
        public static IProteoformModificationLookup CreateFromModifications(IEnumerable<XlmodTerm> modifications,
            IElementProvider elementProvider)
        {
            var lookup = new XlModModificationLookup(elementProvider);

            lookup.SetupModificationArray(modifications);

            return lookup;
        }

        /// <summary>
        /// Gets the chemical formula.
        /// </summary>
        /// <param name="modification">The modification.</param>
        /// <returns></returns>
        protected override IChemicalFormula? GetChemicalFormula(XlmodTerm modification)
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
            if (value.StartsWith("XLMOD:"))
                return value[6..];

            return value;
        }
    }
}