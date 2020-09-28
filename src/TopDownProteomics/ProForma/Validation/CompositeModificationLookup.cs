using System.Collections.Generic;
using System.Linq;
using TopDownProteomics.Proteomics;

namespace TopDownProteomics.ProForma.Validation
{
    /// <summary>
    /// Lookup that delegates to multiple other lookups.
    /// </summary>
    /// <seealso cref="IProteoformModificationLookup" />
    public class CompositeModificationLookup : IProteoformModificationLookup
    {
        private IList<IProteoformModificationLookup> _lookups;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeModificationLookup"/> class.
        /// </summary>
        /// <param name="lookups">The lookups.</param>
        public CompositeModificationLookup(IList<IProteoformModificationLookup> lookups)
        {
            _lookups = lookups;
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
            return _lookups.Any(x => x.CanHandleDescriptor(descriptor));
        }

        /// <summary>
        /// Gets the modification.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <returns></returns>
        public IProteoformModification? GetModification(ProFormaDescriptor descriptor)
        {
            foreach (var lookup in _lookups)
            {
                if (lookup.CanHandleDescriptor(descriptor))
                    return lookup.GetModification(descriptor);
            }

            throw new ProteoformGroupCreateException($"Couldn't handle value for descriptor {descriptor.ToString()}.");
        }
    }
}