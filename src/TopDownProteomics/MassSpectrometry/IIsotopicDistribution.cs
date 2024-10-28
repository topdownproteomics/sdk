using System;
using System.Collections.Generic;

namespace TopDownProteomics.MassSpectrometry;

/// <summary>
/// Neutral distribution of isotopes in a mass spectrometer.
/// </summary>
public interface IIsotopicDistribution
{
    /// <summary>
    /// Gets the length.
    /// </summary>
    int Length { get; }

    /// <summary>The monoisotopic mass.</summary>
    double MonoisotopicMass { get; }

    /// <summary>
    /// Gets the masses.
    /// </summary>
    IList<double> Masses { get; }

    /// <summary>
    /// Gets the intensities.
    /// </summary>
    IList<double> Intensities { get; }

    /// <summary>
    /// Creates a charged isotopic distribution.
    /// </summary>
    /// <param name="charge">The charge.</param>
    /// <param name="positiveCharge">if set to <c>true</c> [positive charge].</param>
    /// <returns>A charged isotopic distribution with the same abundances.</returns>
    [Obsolete("Use CreateChargedDistribution(int charge, double chargeCarrier) instead.")]
    IChargedIsotopicDistribution CreateChargedDistribution(int charge, bool positiveCharge);

    /// <summary>
    /// Creates a charged isotopic distribution.
    /// </summary>
    /// <param name="charge">The charge.</param>
    /// <param name="chargeCarrier">The charge carrier.</param>
    /// <returns>A charged isotopic distribution with the same abundances.</returns>
    IChargedIsotopicDistribution CreateChargedDistribution(int charge, double chargeCarrier = Utility.Proton);

    /// <summary>
    /// Clones the distribution and shifts it by a mass (Da) value.
    /// </summary>
    /// <param name="shift">The shift mass in daltons (Da).</param>
    /// <returns>The cloned distribution shifted by the specified mass.</returns>
    IIsotopicDistribution CloneAndShift(double shift);
}