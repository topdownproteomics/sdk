using TopDownProteomics.Chemistry;

namespace TopDownProteomics.Proteomics
{
    /// <summary>A change in chemical formula that has been applied to a proteoform.</summary>
    /// <seealso cref="IHasChemicalFormula" />
    /// <seealso cref="IProteoformMassDelta" />
    public interface IProteoformFormulaProteoformDelta : IHasChemicalFormula, IProteoformMassDelta { }
}