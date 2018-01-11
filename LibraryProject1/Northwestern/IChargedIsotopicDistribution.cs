namespace TestLibNamespace.Northwestern
{
    /// <summary>
    /// Isotopic Distribution with a given charge in m/z space.
    /// </summary>
    public interface IChargedIsotopicDistribution : IMzIntensityData
    {
        /// <summary>
        /// Gets the charge.
        /// </summary>
        int Charge { get; }

        /// <summary>
        /// Clones the distribution with a subset of the most intense points.
        /// </summary>
        /// <param name="numberOfPoints">The number of points to keep.</param>
        /// <returns></returns>
        IChargedIsotopicDistribution CloneWithMostIntensePoints(int numberOfPoints);

        /// <summary>
        /// Clones the distribution and shifts it by an m/z (Th) value.
        /// </summary>
        /// <param name="shiftMz">The shift m/z in thomsons (Th).</param>
        /// <returns></returns>
        IChargedIsotopicDistribution CloneAndShift(double shiftMz);
    }
}