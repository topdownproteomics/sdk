using TopDownProteomics.ProForma;

namespace TopDownProteomics.Proteomics
{
    /// <summary>
    /// Lookup that handles a certain key and then ignores the descriptor (by returning null).
    /// </summary>
    /// <seealso cref="IProteoformModificationLookup" />
    public class IgnoreKeyModificationLookup : IProteoformModificationLookup
    {
        private ProFormaKey _key;

        /// <summary>
        /// Initializes a new instance of the <see cref="IgnoreKeyModificationLookup"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public IgnoreKeyModificationLookup(ProFormaKey key)
        {
            _key = key;
        }

        /// <summary>
        /// Determines whether this instance [can handle descriptor] the specified descriptor.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <returns>
        /// <c>true</c> if this instance [can handle descriptor] the specified descriptor; otherwise, <c>false</c>.
        /// </returns>
        public bool CanHandleDescriptor(ProFormaDescriptor descriptor)
        {
            return descriptor.Key == _key;
        }

        /// <summary>
        /// Gets the modification.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <returns></returns>
        public IProteoformModification GetModification(ProFormaDescriptor descriptor) => null;
    }
}