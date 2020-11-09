namespace TopDownProteomics.Proteomics
{
    /// <summary>Modification on a chemical proteoform that has a zero-based index.</summary>
    /// <seealso cref="IProteoformModificationWithIndex" />
    public class ProteoformModificationWithIndex : IProteoformModificationWithIndex
    {
        /// <summary>Initializes a new instance of the <see cref="ProteoformModificationWithIndex"/> class.</summary>
        /// <param name="proteoformModification">The proteoform modification.</param>
        /// <param name="zeroBasedIndex">  zero-based index.</param>
        public ProteoformModificationWithIndex(IProteoformModification proteoformModification, int zeroBasedIndex)
        {
            this.Modification = proteoformModification;
            this.ZeroBasedIndex = zeroBasedIndex;
        }

        /// <summary>Gets the zero-based index in the sequence.</summary>
        public int ZeroBasedIndex { get; }

        /// <summary>The modification.</summary>
        public IProteoformModification Modification { get; }
    }
}