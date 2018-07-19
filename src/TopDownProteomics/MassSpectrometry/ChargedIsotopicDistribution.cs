using System.Linq;

namespace TopDownProteomics.MassSpectrometry
{
    /// <summary>
    /// Isotopic Distribution with a given charge.
    /// </summary>
    /// <seealso cref="IMzIntensityData" />
    public class ChargedIsotopicDistribution : IMzIntensityData, IChargedIsotopicDistribution
    {
        private readonly double[] _intensity;
        private readonly double[] _mz;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChargedIsotopicDistribution"/> class.
        /// </summary>
        /// <param name="mz">The mz.</param>
        /// <param name="intensity">The intensity.</param>
        /// <param name="charge">The charge.</param>
        public ChargedIsotopicDistribution(double[] mz, double[] intensity, int charge)
        {
            _intensity = intensity;
            _mz = mz;
            this.Charge = charge;
        }

        /// <summary>
        /// Gets the charge.
        /// </summary>
        public int Charge { get; }

        /// <summary>
        /// Gets the mz.
        /// </summary>
        public double[] GetMz() => _mz;

        /// <summary>
        /// Gets the first m/z.
        /// </summary>
        public double FirstMz => _mz[0];

        /// <summary>
        /// Gets the last m/z.
        /// </summary>
        public double LastMz => _mz[_mz.Length - 1];

        /// <summary>
        /// Gets the intensity.
        /// </summary>
        public double[] GetIntensity() => _intensity;

        /// <summary>
        /// Gets the length.
        /// </summary>
        public int Length => _mz.Length;

        /// <summary>
        /// Clones the distribution with a subset of the most intense points.
        /// </summary>
        /// <param name="numberOfPoints">The number of points to keep.</param>
        /// <returns></returns>
        public IChargedIsotopicDistribution CloneWithMostIntensePoints(int numberOfPoints)
        {
            //for (int i = 0; i < this.Mz.Count - numberOfPoints; i++)
            for (int i = 0; i < _mz.Length - numberOfPoints; i++)
            {
                if (_intensity[i] > _intensity[i + numberOfPoints])
                {
                    // Moving any more would only make things less intense ... stop
                    return new ChargedIsotopicDistribution(_mz.SubSequence(i, i + numberOfPoints - 1).ToArray(),
                        _intensity.SubSequence(i, i + numberOfPoints - 1).ToArray(), this.Charge);
                }
            }

            return null;
        }

        /// <summary>
        /// Clones the distribution and shifts it by an m/z (Th) value.
        /// </summary>
        /// <param name="shiftMz">The shift m/z in thomsons (Th).</param>
        /// <returns></returns>
        public IChargedIsotopicDistribution CloneAndShift(double shiftMz)
        {
            double[] mz = new double[this.Length];

            for (int i = 0; i < this.Length; i++)
            {
                mz[i] = _mz[i] + shiftMz;
            }

            return new ChargedIsotopicDistribution(mz, (double[])_intensity.Clone(), this.Charge);
        }
    }
}