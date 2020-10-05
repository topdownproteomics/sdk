namespace TopDownProteomics.Proteomics
{
    /// <summary>
    /// Modification on a chemical proteoform that has a zero-based index.
    /// </summary>
    /// <seealso cref="IProteoformModification" />
    public interface IProteoformModificationWithIndex : IProteoformModification
    {
        /// <summary>
        /// Gets the zero-based index in the sequence.
        /// </summary>
        int ZeroBasedIndex { get; }
    }
}