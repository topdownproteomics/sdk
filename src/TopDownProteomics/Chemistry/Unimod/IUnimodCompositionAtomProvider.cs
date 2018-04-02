namespace TopDownProteomics.Chemistry.Unimod
{
    /// <summary>
    /// Provides Unimod atoms.
    /// </summary>
    public interface IUnimodCompositionAtomProvider
    {
        /// <summary>
        /// Gets the unimod composition atom.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns></returns>
        UnimodCompositionAtom GetUnimodCompositionAtom(string symbol);
    }
}