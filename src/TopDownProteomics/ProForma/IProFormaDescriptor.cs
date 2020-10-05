namespace TopDownProteomics.ProForma
{
    /// <summary>
    /// Anything that describes a modification.
    /// </summary>
    public interface IProFormaDescriptor
    {
        /// <summary>The key.</summary>
        ProFormaKey Key { get; }

        /// <summary>The type of the evidence.</summary>
        ProFormaEvidenceType EvidenceType { get; }

        /// <summary>The value.</summary>
        string Value { get; }
    }
}