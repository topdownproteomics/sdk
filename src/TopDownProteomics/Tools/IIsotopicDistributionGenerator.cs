using System.Collections.Generic;
using TopDownProteomics.Chemistry;
using TopDownProteomics.MassSpectrometry;

namespace TopDownProteomics.Tools
{
    /// <summary>
    /// Generates both neutral mass and charged isotopic distributions.
    /// </summary>
    public interface IIsotopicDistributionGenerator
    {
        /// <summary>
        /// Generates a neutral mass isotopic distribution.
        /// </summary>
        /// <param name="chemicalFormula">The chemical formula.</param>
        /// <returns></returns>
        IIsotopicDistribution GenerateIsotopicDistribution(ChemicalFormula chemicalFormula);

        /// <summary>
        /// Generates a charged isotopic distribution.
        /// </summary>
        /// <param name="chemicalFormula">The chemical formula.</param>
        /// <param name="charge">The charge.</param>
        /// <returns></returns>
        IChargedIsotopicDistribution GenerateChargedIsotopicDistribution(ChemicalFormula chemicalFormula, int charge);

        /// <summary>
        /// Generates a collection of charged isotopic distributions for a given charge range.
        /// </summary>
        /// <param name="chemicalFormula">The chemical formula.</param>
        /// <param name="firstCharge">The first charge.</param>
        /// <param name="lastCharge">The last charge.</param>
        /// <returns></returns>
        IList<IChargedIsotopicDistribution> GenerateChargedIsotopicDistributions(ChemicalFormula chemicalFormula,
            int firstCharge, int lastCharge);
    }
}