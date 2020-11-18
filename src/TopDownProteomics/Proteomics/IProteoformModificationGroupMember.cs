namespace TopDownProteomics.Proteomics
{
    /// <summary>A member of a modification group.</summary>
    /// <seealso cref="IProteoformLocalizedModification" />
    public interface IProteoformModificationGroupMember : IProteoformLocalizedModification
    {
        /// <summary>The weight of the member.</summary>
        double Weight { get; }
    }
}