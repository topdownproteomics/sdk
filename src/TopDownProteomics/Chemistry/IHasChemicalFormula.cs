namespace TopDownProteomics.Chemistry
{
    /// <summary>
    /// Anything for which a chemical formula can be calculated.
    /// </summary>
    public interface IHasChemicalFormula
    {
        /// <summary>
        /// Gets the chemical formula.
        /// </summary>
        /// <returns></returns>
        IChemicalFormula GetChemicalFormula();
    }
}