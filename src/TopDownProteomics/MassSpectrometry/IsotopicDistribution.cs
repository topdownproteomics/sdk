using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TopDownProteomics.MassSpectrometry
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
#pragma warning disable CS0618 // Type or member is obsolete
                mz[i] = Utility.ConvertMassToMz(_masses[i], charge, positiveCharge);
#pragma warning restore CS0618 // Type or member is obsolete

            return mz;
        }

        private double[] GetMz(int charge, double chargeCarrier)
        {
            double[] mz = new double[this.Length];

            for (int i = 0; i < this.Length; i++)
                mz[i] = Utility.ConvertMassToMz(_masses[i], charge, chargeCarrier);

            return mz;
        }

        /// <summary>
        /// Creates a charged distribution.
        /// </summary>
        /// <param name="charge">The charge.</param>
        /// <param name="positiveCharge">if set to <c>true</c> [positive charge].</param>
        /// <returns></returns>
        public IChargedIsotopicDistribution CreateChargedDistribution(int charge, bool positiveCharge)
        {
            Debug.Assert(charge > 0, "Charge must be greater than 0.");

            return new ChargedIsotopicDistribution(this.GetMz(charge, positiveCharge), _abundances, charge);
        }

        /// <summary>
        /// Creates a charged isotopic distribution.
        /// </summary>
        /// <param name="charge">The charge.</param>
        /// <param name="chargeCarrier">The charge carrier.</param>
        /// <returns>
        /// A charged isotopic distribution with the same abundances.
        /// </returns>
        public IChargedIsotopicDistribution CreateChargedDistribution(int charge, double chargeCarrier = Utility.Proton)
        {
            Debug.Assert(chargeCarrier > 0, "Charge carrier must be greater than 0.");

            return new ChargedIsotopicDistribution(this.GetMz(charge, chargeCarrier), _abundances, charge);
        }

        /// <summary>
        /// Clones the distribution and shifts it by a constant m/z value.
        /// </summary>
        /// <param name="shift">The shift.</param>
        /// <returns></returns>
        public IIsotopicDistribution CloneAndShift(double shift)
        {
            return new IsotopicDistribution(_masses.Select(x => x + shift), new List<double>(_abundances));
        }
    }
}