namespace TopDownProteomics.Proteomics
{
    /// <summary>A proteoform modification that has been localized to some region of the proteoform.</summary>
    public interface IProteoformLocalizedModification : IProteoformModification
    {
        /// <summary>The zero-based start index in the sequence.</summary>
        int ZeroBasedStartIndex { get; }

        /// <summary>Tthe zero-based end index in the sequence.</summary>
        int ZeroBasedEndIndex { get; }
    }
}