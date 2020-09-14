using System.Collections.Generic;

namespace TopDownProteomics.ProForma
{
    /// <summary>
    /// The specified way of writing a modification. Everything between ‘[‘ and ‘]’ (inclusive). A collection of descriptors.
    /// </summary>
    public class ProFormaTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProFormaTag"/> class.
        /// </summary>
        /// <param name="zeroBasedIndex">The zero-based index of the modified amino acid in the sequence.</param>
        /// <param name="descriptors">The descriptors.</param>
        public ProFormaTag(int zeroBasedIndex, IList<ProFormaDescriptor> descriptors)
        {
            ZeroBasedIndex = zeroBasedIndex;
            Descriptors = descriptors;
        }

        /// <summary>Gets the zero-based index in the sequence.</summary>
        public int ZeroBasedIndex { get; }

        /// <summary>Gets the descriptors.</summary>
        public IList<ProFormaDescriptor> Descriptors { get; }
    }
}