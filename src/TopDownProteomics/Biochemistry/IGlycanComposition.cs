using System.Collections.Generic;
using TopDownProteomics.Chemistry;

namespace TopDownProteomics.Biochemistry;

/// <summary>A collection of glycan residues and their cardinalities.</summary>
/// <seealso cref="IHasChemicalFormula" />
public interface IGlycanComposition : IHasChemicalFormula
{
    /// <summary>Gets the elements.</summary>
    IReadOnlyCollection<IEntityCardinality<IGlycanResidue>> GetResidues();
}