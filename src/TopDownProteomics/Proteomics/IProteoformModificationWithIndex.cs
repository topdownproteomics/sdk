namespace TopDownProteomics.Proteomics
{
    /// <summary>Modification on a chemical proteoform that has a zero-based index.</summary>
    /// <seealso cref="IProteoformModification" />
    public interface IProteoformModificationWithIndex
    {
        /// <summary>The zero-based index in the sequence.</summary>
        int ZeroBasedIndex { get; }

        /// <summary>The modification.</summary>
        IProteoformModification Modification { get; }
    }
}