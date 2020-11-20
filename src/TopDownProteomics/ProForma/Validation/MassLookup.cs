using TopDownProteomics.Chemistry;
using TopDownProteomics.Proteomics;

namespace TopDownProteomics.ProForma.Validation
{
    /// <summary>Lookup for modifications given by mass value.</summary>
    /// <seealso cref="IProteoformModificationLookup" />
    public class MassLookup : IProteoformModificationLookup
    {
        /// <summary>Initializes a new instance of the <see cref="MassLookup"/> class.</summary>
        public MassLookup() { }

        /// <summary>Determines whether this instance [can handle descriptor] the specified descriptor.</summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can handle descriptor] the specified descriptor; otherwise, <c>false</c>.</returns>
        public bool CanHandleDescriptor(IProFormaDescriptor descriptor)
        {
            return descriptor.Key == ProFormaKey.Mass;
        }

        /// <summary>Gets the modification.</summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <returns></returns>
        public IProteoformMassDelta? GetModification(IProFormaDescriptor descriptor)
        {
            if (double.TryParse(descriptor.Value, out double mass))
            {
                return new MassModification(mass);
            }
            else
            {
                throw new ProteoformModificationLookupException($"Could not parse mass string for descriptor {descriptor}");
            }
        }

        private class MassModification : IProteoformMassDelta
        {
            private readonly double _mass;

            public MassModification(double mass)
            {
                _mass = mass;
            }

            public double GetMass(MassType massType) => _mass;
        }
    }
}