namespace TopDownProteomics.Biochemistry;

/// <summary>Provides glycan residues.</summary>
public interface IGlycanResidueProvider
{
    /// <summary>Gets the glycan residue for a given symbol.</summary>
    /// <param name="symbol">The symbol.</param>
    IGlycanResidue GetGlycanResidue(string symbol);
}