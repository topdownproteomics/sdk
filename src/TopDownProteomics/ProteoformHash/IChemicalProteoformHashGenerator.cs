namespace TopDownProteomics.ProteoformHash
{
    /// <summary>
    /// Handles the generation of hash codes surrounding proteoforms in the ProForma notation.
    /// </summary>
    public interface IChemicalProteoformHashGenerator
    {
        /// <summary>
        /// Generates a proteoform hash from a ProForma string.
        /// </summary>
        /// <param name="proForma">The ProForma string.</param>
        /// <returns></returns>
        IChemicalProteoformHash Generate(string proForma);
    }
}