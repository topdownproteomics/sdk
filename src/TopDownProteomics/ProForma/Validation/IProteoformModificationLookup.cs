using TopDownProteomics.Proteomics;

namespace TopDownProteomics.ProForma.Validation
{
    /// <summary>
    /// Looks up a proteoform modification based on a ProForma descriptor.
    /// </summary>
    public interface IProteoformModificationLookup
    {
        /// <summary>
        /// Determines whether this instance [can handle descriptor] the specified descriptor.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can handle descriptor] the specified descriptor; otherwise, <c>false</c>.
        /// </returns>
        bool CanHandleDescriptor(ProFormaDescriptor descriptor);

        /// <summary>
        /// Gets the modification.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <returns></returns>
        IProteoformModification GetModification(ProFormaDescriptor descriptor);
    }
}