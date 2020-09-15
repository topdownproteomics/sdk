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
            this.ZeroBasedStartIndex = zeroBasedIndex;
            this.ZeroBasedEndIndex = zeroBasedIndex;
            this.Descriptors = descriptors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProFormaTag" /> class.
        /// </summary>
        /// <param name="zeroBasedStartIndex">The zero-based start index of the modified amino acid in the sequence.</param>
        /// <param name="zeroBasedEndIndex">The zero-based end index of the modified amino acid in the sequence.</param>
        /// <param name="descriptors">The descriptors.</param>
        public ProFormaTag(int zeroBasedStartIndex, int zeroBasedEndIndex, IList<ProFormaDescriptor> descriptors)
        {
            this.ZeroBasedStartIndex = zeroBasedStartIndex;
            this.ZeroBasedEndIndex = zeroBasedEndIndex;
            this.Descriptors = descriptors;
        }

        /// <summary>Gets the zero-based start index in the sequence.</summary>
        public int ZeroBasedStartIndex { get; }

        /// <summary>Gets the zero-based end index in the sequence.</summary>
        public int ZeroBasedEndIndex { get; }

        /// <summary>Gets the descriptors.</summary>
        public IList<ProFormaDescriptor> Descriptors { get; }
    }
}