namespace TopDownProteomics.ProForma
{
    /// <summary>A member of a tag group.</summary>
    public class ProFormaMembershipDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProFormaMembershipDescriptor"/> class.
        /// </summary>
        /// <param name="zeroBasedIndex">The zero-based index of the modified amino acid in the sequence.</param>
        /// <param name="weight">The weight.</param>
        public ProFormaMembershipDescriptor(int zeroBasedIndex, double weight = 0.0)
        {
            this.ZeroBasedStartIndex = zeroBasedIndex;
            this.ZeroBasedEndIndex = zeroBasedIndex;
            this.Weight = weight;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProFormaMembershipDescriptor"/> class.
        /// </summary>
        /// <param name="zeroBasedStartIndex">The zero-based start index of the modified amino acid in the sequence.</param>
        /// <param name="zeroBasedEndIndex">The zero-based end index of the modified amino acid in the sequence.</param>
        /// <param name="weight">The weight.</param>
        public ProFormaMembershipDescriptor(int zeroBasedStartIndex, int zeroBasedEndIndex, double weight = 0.0)
        {
            this.ZeroBasedStartIndex = zeroBasedStartIndex;
            this.ZeroBasedEndIndex = zeroBasedEndIndex;
            this.Weight = weight;
        }

        /// <summary>Gets the zero-based start index in the sequence.</summary>
        public int ZeroBasedStartIndex { get; }

        /// <summary>Gets the zero-based end index in the sequence.</summary>
        public int ZeroBasedEndIndex { get; }

        /// <summary>The weight this member has on the group.</summary>
        public double Weight { get; }
    }
}