using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TopDownProteomics.Chemistry;
using TopDownProteomics.MassSpectrometry;

namespace TopDownProteomics.Tools
{
    /// <summary>
    /// A .NET implementation of the Mercury7 algorithm. Analagous data structures are used whenever possible, 
    /// e.g., std::vector{double}; replaced with List{double}.
    /// 
    /// Heavily modified by PMThomas 13 April 2012.
    /// 
    /// NOTE THE LIBMERCURY C++ LICENSE, LGPLv2! - SEE LIBMERCURY++ COMMENTS.
    /// 
    /// RTF: I think this is the link, but not confirmed: http://fiehnlab.ucdavis.edu/projects/Seven_Golden_Rules/Isotopic_Pattern_Generator
    /// RTF: Paper: Efficient calculation of accurate masses of isotopic peaks. JASMS 2006, Rockwood AL, Haimi P.
    /// </summary>
    public class Mercury7 : IIsotopicDistributionGenerator
    {
        private readonly double _limit;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mercury7"/> class.
        /// </summary>
        /// <param name="limit">The limit.</param>
        public Mercury7(double limit = 1E-26)
        {
            _limit = limit;
        }

        /// <summary>
        /// Calculates the expected isotopic distribution for a given composition.
        /// </summary>
        /// <param name="chemicalFormula">The chemical formula.</param>
        /// <returns></returns>
        public IIsotopicDistribution GenerateIsotopicDistribution(ChemicalFormula chemicalFormula)
        {
            return this.Mercury(chemicalFormula, this._limit);
        }

        /// <summary>
        /// Calculates the expected charged isotopic distribution for a given composition and charge.
        /// </summary>
        /// <param name="chemicalFormula">The chemical formula.</param>
        /// <param name="charge">The charge.</param>
        /// <returns></returns>
        public IChargedIsotopicDistribution GenerateChargedIsotopicDistribution(ChemicalFormula chemicalFormula, int charge)
        {
            return this.Mercury(chemicalFormula, charge, this._limit);
        }

        /// <summary>
        /// Generates a list of charged isotopic distributions using the Mercury algorithm.
        /// </summary>
        /// <param name="chemicalFormula">The chemical formula.</param>
        /// <param name="firstCharge">The first charge.</param>
        /// <param name="lastCharge">The last charge.</param>
        /// <returns></returns>
        public IList<IChargedIsotopicDistribution> GenerateChargedIsotopicDistributions(ChemicalFormula chemicalFormula, int firstCharge, int lastCharge)
        {
            return this.Mercury(chemicalFormula, firstCharge, lastCharge, this._limit);
        }

        private IChargedIsotopicDistribution Mercury(ChemicalFormula cf, int charge, double limit)
        {
            return this.Mercury(cf, charge, charge, limit).Single();
        }
        private List<IChargedIsotopicDistribution> Mercury(ChemicalFormula cf, int firstcharge, int lastcharge, double limit)
        {
            IIsotopicDistribution dist = this.Mercury(cf, limit);
            var mercury7ResultList = new List<IChargedIsotopicDistribution>();

            if (firstcharge > 0)
            {
                for (int j = firstcharge; j < lastcharge + 1; j++)
                {
                    mercury7ResultList.Add(dist.CreateChargedDistribution(j));
                }
            }
            else if (firstcharge < 0)
            {
                for (int j = firstcharge; j > lastcharge - 1; j--)
                {
                    mercury7ResultList.Add(dist.CreateChargedDistribution(j, false));
                }
            }

            return mercury7ResultList;
        }
        private IIsotopicDistribution Mercury(ChemicalFormula cf, double limit)
        {
            // Build up the molecular super atom (MSA) until it is the entire molecule
            // A "molecular super atom" refers to a fictitious chemical compound whose 
            //  formula is a partial composition of the target compound.
            double[]? msaMz = null;
            double[]? msaAbundance = null;

            double[]? tmpMz = null;
            double[]? tmpAbundance = null;
            bool msaInitialized = false;

            foreach (IEntityCardinality<IElement> kvp in cf.GetElements())
            {
                uint n = (uint)kvp.Count;

                if (n == 0)
                    continue;

                int isotopeCount = kvp.Entity.Isotopes.Count;
                double[] esaMz = new double[isotopeCount];
                double[] esaAbundance = new double[isotopeCount];

                int i = 0;
                foreach (var iso in kvp.Entity.Isotopes.OrderBy(x => x.AtomicMass)) // Algorithm requires it to be sorted.
                {
                    esaMz[i] = iso.AtomicMass;
                    esaAbundance[i] = iso.RelativeAbundance;
                    i++;
                }

                while (true)
                {
                    // This is an implicit FFT that decomposes the number of a particular element
                    // into the sum of its powers of 2.
                    // Check if we need to do the MSA update - only if n is odd
                    if ((n & 1) == 1)
                    {
                        // MSA update
                        if (msaInitialized)
                        {
                            // normal update
                            Convolve(ref tmpMz, ref tmpAbundance, msaMz, msaAbundance, esaMz, esaAbundance);

                            msaMz = this.CopyArray(tmpMz);
                            msaAbundance = this.CopyArray(tmpAbundance);
                        }
                        else
                        {
                            // for first assignment, MSA = ESA
                            msaMz = this.CopyArray(esaMz);
                            msaAbundance = this.CopyArray(esaAbundance);
                            msaInitialized = true;
                        }

                        Prune(ref msaMz, ref msaAbundance, limit);
                    }

                    // The ESA update is always carried out (with the exception of the last time, i.e., when n == 1)
                    if (n == 1)
                        break;

                    Convolve(ref tmpMz, ref tmpAbundance, esaMz, esaAbundance, esaMz, esaAbundance);

                    esaMz = this.CopyArray(tmpMz);
                    esaAbundance = this.CopyArray(tmpAbundance);

                    Prune(ref esaMz, ref esaAbundance, limit);
                    n = n >> 1;
                }
            }

            if (msaMz == null || msaAbundance == null)
                throw new Exception("msa Arrays must not be empty.");

            return new IsotopicDistribution(msaMz, msaAbundance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double[] CopyArray(double[]? source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            double[] target = new double[source.Length];
            Array.Copy(source, target, source.Length);

            return target;
        }

        private void Prune(ref double[] mz, ref double[] ab, double limit)
        {
            if (mz.Length == 0)
                return;

            int start = 0;
            int end = mz.Length - 1;

            while (ab[start] < limit && start != end)
                start++;

            while (end >= start && ab[end] < limit)
                end--;

            // See if we need to prune
            if (end - start < mz.Length - 1)
            {
                int length = end - start + 1;
                double[] tmpMz = new double[length];
                double[] tmpAb = new double[length];

                Array.Copy(mz, start, tmpMz, 0, length);
                Array.Copy(ab, start, tmpAb, 0, length);

                mz = tmpMz;
                ab = tmpAb;
            }
        }
        private void Convolve(ref double[]? resultMz, ref double[]? resultAb, double[]? mz1, double[]? ab1,
            double[] mz2, double[] ab2)
        {
            // RTF ignoring here for speed ... should look into changing the logic to avoid this
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            int n1 = mz1.Length;
            int n2 = mz2.Length;

            if (n1 + n2 == 0)
                return;

            // Adaptation of speed optimization from C++...may not do anything similar in C#
            resultMz = new double[n1 + n2];
            resultAb = new double[n1 + n2];

            // For each isotopic peak in the compound...
            for (int k = 0; k < n1 + n2 - 1; k++)
            {
                double totalAbundance = 0;
                double massExpectation = 0;
                int start = k < (n2 - 1) ? 0 : k - n2 + 1; // start = max(0, k - n2 + 1)
                int end = k < (n1 - 1) ? k : n1 - 1; // end = min(n1 - 1, k)

                // Calculate the mass expectation value and the abundance
                for (int i = start; i <= end; i++)
                {
                    double ithAbundance = ab1[i] * ab2[k - i];
                    if (ithAbundance > 0)
                    {
                        totalAbundance += ithAbundance;
                        massExpectation += ithAbundance * (mz1[i] + mz2[k - i]);
                    }
                }

                //Do NOT throw away isotopes with zero probability, this would screw up the isotope count k!
                resultMz[k] = totalAbundance > 0 ? massExpectation / totalAbundance : 0;
                resultAb[k] = totalAbundance;
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }
}