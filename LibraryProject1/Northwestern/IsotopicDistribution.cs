using System.Collections.Generic;
using System.Linq;

namespace TestLibNamespace.Northwestern
{
    /// <summary>
    /// Neutral distribution of isotopes.
    /// </summary>
    public class IsotopicDistribution : IIsotopicDistribution
    {
        private readonly double[] _abundances;
        private readonly double[] _masses;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsotopicDistribution" /> class.
        /// </summary>
        /// <param name="masses">The masses.</param>
        /// <param name="abundances">The abundances.</param>
        public IsotopicDistribution(IList<double> masses, IList<double> abundances)
        {
            _masses = masses.ToArray();
            _abundances = abundances.ToArray();
            this.Length = _masses.Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsotopicDistribution"/> class.
        /// </summary>
        /// <param name="masses">The masses.</param>
        /// <param name="abundances">The abundances.</param>
        public IsotopicDistribution(double[] masses, double[] abundances)
        {
            _masses = masses;
            _abundances = abundances;
            this.Length = _masses.Length;
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Gets the masses.
        /// </summary>
        public IList<double> Masses => _masses;

        /// <summary>
        /// Gets the intensities.
        /// </summary>
        public IList<double> Intensities => _abundances;

        private double[] GetMz(int charge, bool positiveCharge)
        {
            double[] mz = new double[this.Length];

            for (int i = 0; i < this.Length; i++)
                mz[i] = Utility.ConvertMassToMz(_masses[i], charge, positiveCharge);

            return mz;
        }

        /// <summary>
        /// Creates a charged distribution.
        /// </summary>
        /// <param name="charge">The charge.</param>
        /// <param name="positiveCharge">if set to <c>true</c> [positive charge].</param>
        /// <returns></returns>
        public IChargedIsotopicDistribution CreateChargedDistribution(int charge, bool positiveCharge = true)
        {
            return new ChargedIsotopicDistribution(this.GetMz(charge, positiveCharge), _abundances, charge);
        }

        /// <summary>
        /// Clones the distribution and shifts it by .
        /// </summary>
        /// <param name="shift">The shift.</param>
        /// <returns></returns>
        public IIsotopicDistribution CloneAndShift(double shift)
        {
            return new IsotopicDistribution(_masses.Select(x => x + shift), new List<double>(_abundances));
        }
    }
}