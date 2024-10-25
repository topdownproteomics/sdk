using System;
using System.Linq;

namespace TopDownProteomics.MassSpectrometry;

/// <summary>
/// Isotopic Distribution with a given charge.
/// </summary>
/// <seealso cref="IMzIntensityData" />
/// <remarks>
/// Initializes a new instance of the <see cref="ChargedIsotopicDistribution"/> class.
/// </remarks>
/// <param name="monoMz">The mono m/z.</param>
/// <param name="mz">The mz.</param>
/// <param name="intensity">The intensity.</param>
/// <param name="charge">The charge.</param>
/// <param name="chargeCarrier">The charge carrier.</param>
public class ChargedIsotopicDistribution(double monoMz, double[] mz, double[] intensity, int charge, double chargeCarrier) : IMzIntensityData, IChargedIsotopicDistribution
{
    private readonly double[] _intensity = intensity;
    private readonly double[] _mz = mz;

    /// <summary>The charge.</summary>
    public int Charge { get; } = charge;

    /// <summary>The mass of the charge carrier.</summary>
    public double ChargeCarrier { get; } = chargeCarrier;

    /// <summary>
    /// Gets the mz.
    /// </summary>
    public double[] GetMz() => _mz;

    /// <summary>
    /// Gets the first m/z.
    /// </summary>
    public double FirstMz => _mz[0];

    /// <summary>The monoisotopic m/z.</summary>
    public double MonoisotopicMz { get; } = monoMz;

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
        for (int i = 0; i < _mz.Length - numberOfPoints; i++)
        {
            if (_intensity[i] > _intensity[i + numberOfPoints])
            {
                // Moving any more would only make things less intense ... stop
                return new ChargedIsotopicDistribution(this.MonoisotopicMz, _mz.SubSequence(i, i + numberOfPoints - 1).ToArray(),
                    _intensity.SubSequence(i, i + numberOfPoints - 1).ToArray(), this.Charge, this.ChargeCarrier);
            }
        }

        throw new Exception($"Cannot find most {numberOfPoints} intense points.");
    }

    /// <summary>
    /// Clones the distribution and shifts it by an m/z (Th) value.
    /// </summary>
    /// <param name="shiftMz">The shift m/z in Thomsons (Th).</param>
    /// <returns></returns>
    public IChargedIsotopicDistribution CloneAndShift(double shiftMz)
    {
        double[] mz = new double[this.Length];
        double monoMz = this.MonoisotopicMz + shiftMz;

        for (int i = 0; i < this.Length; i++)
        {
            mz[i] = _mz[i] + shiftMz;
        }

        return new ChargedIsotopicDistribution(monoMz, mz, (double[])_intensity.Clone(), this.Charge, this.ChargeCarrier);
    }
}