using TopDownProteomics.Chemistry;

namespace TopDownProteomics.Biochemistry;

/// <summary>A single atom of a glycan composition.</summary>
/// <seealso cref="IHasChemicalFormula" />
public interface IGlycanResidue : IHasChemicalFormula
{
    /// <summary>The name of the glycan residue</summary>
    string Name { get; }

    /// <summary>The glycan residue's symbol</summary>
    string Symbol { get; }
}