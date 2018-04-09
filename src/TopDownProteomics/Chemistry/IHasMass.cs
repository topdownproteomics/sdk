namespace TopDownProteomics.Chemistry
{
    /// <summary>
    /// Anything that has both a monoisotopic and average mass.
    /// </summary>
    public interface IHasMass
    {
        /// <summary>
        /// Gets the mass.
        /// </summary>
        /// <param name="massType">Type of the mass.</param>
        /// <returns></returns>
        double GetMass(MassType massType);
    }
}