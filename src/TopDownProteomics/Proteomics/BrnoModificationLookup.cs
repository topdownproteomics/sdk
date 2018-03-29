using TopDownProteomics.ProForma;

namespace TopDownProteomics.Proteomics
{
    /// <summary>
    /// Lookup for Brno nomenclature for histone modifications.
    /// https://doi.org/10.1038/nsmb0205-110
    /// </summary>
    /// <seealso cref="IProteoformModificationLookup" />
    public class BrnoModificationLookup : IProteoformModificationLookup
    {
        /// <summary>
        /// Determines whether this instance [can handle descriptor] the specified descriptor.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <returns>
        /// <c>true</c> if this instance [can handle descriptor] the specified descriptor; otherwise, <c>false</c>.
        /// </returns>
        public bool CanHandleDescriptor(ProFormaDescriptor descriptor)
        {
            return descriptor.Key == ProFormaKey.Mod && descriptor.Value != null && descriptor.Value.EndsWith("(BRNO)");
        }

        /// <summary>
        /// Gets the modification.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <returns></returns>
        public IProteoformModification GetModification(ProFormaDescriptor descriptor)
        {
            string abbreviation = descriptor.Value.Substring(0, descriptor.Value.IndexOf("("));

            switch (abbreviation)
            {
                case "ac": return new BrnoModification();
                case "me1": return new BrnoModification();
                case "me2s":
                case "me2a":
                case "me2": return new BrnoModification();
                case "me3": return new BrnoModification();
                case "ph": return new BrnoModification();

                default: return null;
            }
        }

        private class BrnoModification : IProteoformModification
        {

        }
    }
}