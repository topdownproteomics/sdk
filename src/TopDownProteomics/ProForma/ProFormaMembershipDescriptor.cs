namespace TopDownProteomics.ProForma
{
    /// <summary>A member of a tag group.</summary>
    public class ProFormaMembershipDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProFormaMembershipDescriptor"/> class.
        /// </summary>
        /// <param name="zeroBasedIndex">Index of the zero based.</param>
        /// <param name="weight">The weight.</param>
        public ProFormaMembershipDescriptor(int zeroBasedIndex, double weight = 0.0)
        {
            this.ZeroBasedIndex = zeroBasedIndex;
            this.Weight = weight;
        }

        /// <summary>The zero-based index in the sequence.</summary>
        public int ZeroBasedIndex { get; }

        /// <summary>The weight this member has on the group.</summary>
        public double Weight { get; }
    }
}