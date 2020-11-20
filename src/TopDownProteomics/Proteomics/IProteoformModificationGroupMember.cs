namespace TopDownProteomics.Proteomics
{
    /// <summary>A member of a modification group.</summary>
    public interface IProteoformModificationGroupMember
    {
        /// <summary>The zero-based start index in the sequence.</summary>
        int ZeroBasedStartIndex { get; }

        /// <summary>Tthe zero-based end index in the sequence.</summary>
        int ZeroBasedEndIndex { get; }

        /// <summary>The weight of the member.</summary>
        double Weight { get; }
    }
}