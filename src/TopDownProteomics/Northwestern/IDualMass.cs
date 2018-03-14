namespace TestLibNamespace.Northwestern
{
    /// <summary>
    /// Any object that stores both Average and Monoisotopic masses.
    /// </summary>
    public interface IDualMass
    {
        /// <summary>
        /// Gets the mass.
        /// </summary>
        /// <param name="massType">The mass type.</param>
        /// <returns></returns>
        double GetMass(MassType massType);
    }
}