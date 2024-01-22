using TopDownProteomics.ProForma;

namespace TopDownProteomics.Proteomics;

/// <summary>A delta modification from an ontology that has been applied to a proteoform.</summary>
/// <seealso cref="IIdentifiable" />
/// <seealso cref="IProteoformFormulaProteoformDelta" />
public interface IProteoformOntologyDelta : IIdentifiable, IProteoformFormulaProteoformDelta
{
    /// <summary>The type of the evidence for this ontology term.</summary>
    ProFormaEvidenceType EvidenceType { get; }
}