using System.Collections.Generic;
using TopDownProteomics.Chemistry;
using TopDownProteomics.Chemistry.Unimod;
using TopDownProteomics.IO.Unimod;

namespace TopDownProteomics.ProForma.Validation
{
    /// <summary>
    /// Lookup for Unimod modifications.
    /// </summary>
    /// <seealso cref="IProteoformModificationLookup" />
    public class UnimodModificationLookup : ModificationLookupBase<UnimodModification>
    {
        private readonly IUnimodCompositionAtomProvider _atomProvider;

        /// <summary>The UNIMOD prefix</summary>
        public readonly static string Prefix = "UNIMOD:";

        private UnimodModificationLookup(IUnimodCompositionAtomProvider atomProvider)
        {
            _atomProvider = atomProvider;
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
        protected override ProFormaEvidenceType EvidenceType => ProFormaEvidenceType.Unimod;

        /// <summary>
        /// Initializes the <see cref="ResidModificationLookup" /> class.
        /// </summary>
        /// <param name="modifications">The modifications.</param>
        /// <param name="atomProvider">The atom provider.</param>
        /// <returns></returns>
        public static IProteoformModificationLookup CreateFromModifications(IEnumerable<UnimodModification> modifications,
            IUnimodCompositionAtomProvider atomProvider)
        {
            var lookup = new UnimodModificationLookup(atomProvider);

            lookup.SetupModificationArray(modifications);

            return lookup;
        }

        /// <summary>
        /// Gets the chemical formula.
        /// </summary>
        /// <param name="modification">The modification.</param>
        /// <returns></returns>
        protected override ChemicalFormula GetChemicalFormula(UnimodModification modification)
        {
            return modification.GetChemicalFormula(_atomProvider);
        }

        /// <summary>
        /// Removes the prefix.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected override string RemovePrefix(string value)
        {
            if (value.StartsWith(Prefix))
                return value.Substring(7);
            
            return value;
        }
    }
}