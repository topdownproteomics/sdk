namespace TopDownProteomics.ProteoformHash
{
    /// <summary>A chemical proteoform hash.</summary>
    public interface IChemicalProteoformHash
    {
        /// <summary>Gets the chemical proteoform hash.</summary>
        string Hash { get; }

        /// <summary>Gets a value indicating whether this instance has a ProForma string.</summary>
        bool HasProForma { get; }

        /// <summary>Gets the ProForma string.</summary>
        string ProForma { get; }
    }
}