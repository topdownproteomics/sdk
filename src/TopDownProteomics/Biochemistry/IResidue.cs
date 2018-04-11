using TopDownProteomics.Chemistry;

namespace TopDownProteomics.Biochemistry
{
    /// <summary>
    /// Represents an individual building block of a larger structure.
    /// </summary>
    public interface IResidue : IHasChemicalFormula
    {
        /// <summary>
        /// The residue name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The residue symbol.
        /// </summary>
        char Symbol { get; }
    }
}