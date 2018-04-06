namespace TopDownProteomics.Chemistry
{
    /// <summary>
    /// Masses are either monoisotopic or average.
    /// </summary>
    public enum MassType
    {
        /// <summary>
        /// Unbound, ground-state, rest mass
        /// </summary>
        Monoisotopic,

        /// <summary>
        /// Average mass of all isotopes
        /// </summary>
        Average
    }
}