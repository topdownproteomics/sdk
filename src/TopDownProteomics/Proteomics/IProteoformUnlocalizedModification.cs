namespace TopDownProteomics.Proteomics
{
    /// <summary>A modification that is not localized to any sub-region of the proteoform.</summary>
    public interface IProteoformUnlocalizedModification
    {
        /// <summary>The number of modifications applied.</summary>
        int Count { get; }

        /// <summary>Indicates whether this instance is a labile modification.</summary>
        bool IsLabile { get; }
    }
}