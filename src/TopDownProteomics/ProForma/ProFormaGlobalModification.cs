using System.Collections.Generic;

namespace TopDownProteomics.ProForma
{
    /// <summary>A modification to applies globally based on a target or targets.</summary>
    public class ProFormaGlobalModification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProFormaGlobalModification"/> class.
        /// </summary>
        /// <param name="descriptors">The descriptors.</param>
        /// <param name="targetAminoAcids">The target amino acids.</param>
        public ProFormaGlobalModification(IList<ProFormaDescriptor> descriptors, ICollection<char>? targetAminoAcids)
        {
            Descriptors = descriptors;
            TargetAminoAcids = targetAminoAcids;
        }

        /// <summary>The descriptors for this global modification.</summary>
        public IList<ProFormaDescriptor> Descriptors { get; }

        /// <summary>The amino acids targeted by this global modification (null if representing isotopes).</summary>
        public ICollection<char>? TargetAminoAcids { get; }
    }
}