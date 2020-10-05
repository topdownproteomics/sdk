using System.Collections.Generic;

namespace TopDownProteomics.ProForma
{
    /// <summary>
    /// The specified way of writing an unlocalized modification. Everything between ‘[‘ and ‘]’ (inclusive) that is followed by a '?'.
    /// </summary>
    public class ProFormaUnlocalizedTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProFormaUnlocalizedTag"/> class.
        /// </summary>
        /// <param name="count">The number of unlocalized modifications applied.</param>
        /// <param name="descriptors">The descriptors.</param>
        public ProFormaUnlocalizedTag(int count, IList<ProFormaDescriptor> descriptors)
        {
            Count = count;
            Descriptors = descriptors;
        }

        /// <summary>The number of unlocalized modifications applied.</summary>
        public int Count { get; }

        /// <summary>Gets the descriptors.</summary>
        public IList<ProFormaDescriptor> Descriptors { get; }
    }
}