namespace TopDownProteomics.MassSpectrometry
{
    /// <summary>
    /// Represents simple spectral data that contains only m/z and intensity.
    /// </summary>
    public interface IMzIntensityData
    {
        /// <summary>
        /// Gets the first m/z.
        /// </summary>
        double FirstMz { get; }

        /// <summary>
        /// Gets the last m/z.
        /// </summary>
        double LastMz { get; }

        /// <summary>
        /// Gets the m/z array.
        /// </summary>
        /// <returns></returns>
        double[] GetMz();

        /// <summary>
        /// Gets the intensity array.
        /// </summary>
        /// <returns></returns>
        double[] GetIntensity();

        /// <summary>
        /// Gets the length.
        /// </summary>
        int Length { get; }
    }
}