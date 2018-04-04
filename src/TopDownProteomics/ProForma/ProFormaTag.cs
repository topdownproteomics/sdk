using System.Collections.Generic;

namespace TopDownProteomics.ProForma
{
    /// <summary>
    /// The specified way of writing a localized modification. Everything between ‘[‘ and ‘]’ (inclusive). A collection of descriptors.
    /// </summary>
    public class ProFormaTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProFormaTag"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="descriptors">The descriptors.</param>
        public ProFormaTag(int index, IList<ProFormaDescriptor> descriptors)
        {
            Index = index;
            Descriptors = descriptors;
        }

        /// <summary>
        /// Gets the zero-based index in the sequence.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Gets the descriptors.
        /// </summary>
        public IList<ProFormaDescriptor> Descriptors { get; }
    }
}