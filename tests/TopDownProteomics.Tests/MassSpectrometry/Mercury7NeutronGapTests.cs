using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TopDownProteomics.Chemistry;
using TopDownProteomics.MassSpectrometry;
using TopDownProteomics.Tools;

namespace TopDownProteomics.Tests.MassSpectrometry;

[TestFixture]
public class Mercury7NeutronGapTests
{
    private readonly IElementProvider _provider = new MercuryTestElementProvider();

    [TestCase("Se")]
    [TestCase("Se2")]
    [TestCase("Fe")]
    [TestCase("Fe2")]
    [TestCase("Xe")]
    [TestCase("Xe2")]
    [TestCase("Br")]
    [TestCase("Zn")]
    [TestCase("C213H320N66O65S2Se")]
    [TestCase("C210H315N65O64S2")]
    public void DistributionMatchesAtomByAtomOracle(string formulaText)
    {
        var formula = ChemicalFormula.ParseString(formulaText.AsSpan(), _provider);
        var expected = AtomByAtomOracle(formula);
        var actual = new Mercury7(0).GenerateIsotopicDistribution(formula);

        Assert.Multiple(() =>
        {
            Assert.AreEqual(expected.Count, actual.Length, $"Peak count for {formulaText}");
            for (int i = 0; i < expected.Count; i++)
            {
                if (expected[i].Abundance >= 1e-8)
                {
                    Assert.AreEqual(expected[i].Mass, actual.Masses[i], 1e-6, $"Mass at peak {i} for {formulaText}");
                    Assert.AreEqual(expected[i].Abundance, actual.Intensities[i], 1e-10, $"Intensity at peak {i} for {formulaText}");
                }
                Assert.Greater(actual.Masses[i], 0, $"Positive mass at peak {i} for {formulaText}");
                if (i > 0)
                    Assert.Greater(actual.Masses[i], actual.Masses[i - 1], $"Mass ordering for {formulaText}");
            }
        });
    }

    [Test]
    public void SeleniumMonoisotopicMetadataRemainsMostAbundantIsotope()
    {
        var formula = ChemicalFormula.ParseString("Se".AsSpan(), _provider);
        var result = new Mercury7(0).GenerateIsotopicDistribution(formula);
        var selenium = _provider.GetElement("Se".AsSpan());

        Assert.AreEqual(selenium.Isotopes.Single(x => x.NeutronCount == 46).AtomicMass, result.MonoisotopicMass, 1e-12);
        Assert.AreEqual(selenium.Isotopes.Min(x => x.AtomicMass), result.Masses[0], 1e-12);
        Assert.Greater(result.MonoisotopicMass, result.Masses[0]);
    }

    [TestCase(1)]
    [TestCase(-1)]
    [TestCase(2)]
    [TestCase(-2)]
    [TestCase(6)]
    [TestCase(-6)]
    public void ChargedDistributionPreservesSparsePeakMassesAndCarrier(int charge)
    {
        const double carrier = 2.3456789;
        var formula = ChemicalFormula.ParseString("Se2".AsSpan(), _provider);
        var neutral = new Mercury7(0).GenerateIsotopicDistribution(formula);
        var charged = new Mercury7(0, carrier).GenerateChargedIsotopicDistribution(formula, charge);
        var chargedMz = charged.GetMz();
        var chargedIntensity = charged.GetIntensity();

        Assert.AreEqual(neutral.Length, charged.Length);
        for (int i = 0; i < neutral.Length; i++)
        {
            Assert.AreEqual((neutral.Masses[i] + charge * carrier) / Math.Abs(charge), chargedMz[i], 1e-10);
            Assert.AreEqual(neutral.Intensities[i], chargedIntensity[i], 1e-12);
        }
    }

    [Test]
    public void UnorderedSparseZeroAndSingleIsotopeInputsAreHandled()
    {
        var sparse = new Element(100, "Qq", new IIsotope[]
        {
            new Isotope(105.125, 15, 0.25),
            new Isotope(100.125, 10, 0.5),
            new Isotope(102.125, 12, 0),
            new Isotope(104.125, 14, 0.25)
        });
        var fixedIsotope = new Element(101, "Rr", new IIsotope[]
        {
            new Isotope(203.456, 102, 1)
        });
        var sparseFormula = new ChemicalFormula(new[] { new EntityCardinality<IElement>(sparse, 2) });
        var fixedFormula = new ChemicalFormula(new[] { new EntityCardinality<IElement>(fixedIsotope, 3) });

        AssertMatchesOracle(sparseFormula);
        AssertMatchesOracle(fixedFormula);
    }

    [Test]
    public void CutoffAboveAllProbabilitiesProducesAnEmptyDistribution()
    {
        var formula = ChemicalFormula.ParseString("Se".AsSpan(), _provider);
        var result = new Mercury7(1.1).GenerateIsotopicDistribution(formula);
        Assert.AreEqual(0, result.Length);
        Assert.AreEqual(formula.GetMass(MassType.Monoisotopic), result.MonoisotopicMass, 1e-12);
    }

    [Test]
    public void ChargedDistributionRangeKeepsChargeMetadata()
    {
        var formula = ChemicalFormula.ParseString("Se".AsSpan(), _provider);
        var results = new Mercury7(0, 2.3456789).GenerateChargedIsotopicDistributions(formula, 1, 3);

        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, results.Select(x => x.Charge));
        foreach (var result in results)
            Assert.AreEqual(formula.GetMass(MassType.Monoisotopic) / Math.Abs(result.Charge) + Math.Sign(result.Charge) * 2.3456789,
                result.MonoisotopicMz, 1e-10);
    }

    [Test]
    public void PracticalCutoffRemovesOnlyLowProbabilityEdgePeaks()
    {
        const double cutoff = 0.1;
        var formula = ChemicalFormula.ParseString("Se2".AsSpan(), _provider);
        var full = new Mercury7(0).GenerateIsotopicDistribution(formula);
        var trimmed = new Mercury7(cutoff).GenerateIsotopicDistribution(formula);

        Assert.Less(trimmed.Length, full.Length);
        Assert.GreaterOrEqual(trimmed.Intensities[0], cutoff);
        Assert.GreaterOrEqual(trimmed.Intensities[trimmed.Length - 1], cutoff);
        Assert.Less(full.Intensities[0], cutoff);
        Assert.Less(full.Intensities[full.Length - 1], cutoff);

        int fullIndex = Array.FindIndex(full.Masses.ToArray(), mass => Math.Abs(mass - trimmed.Masses[0]) < 1e-7);
        Assert.GreaterOrEqual(fullIndex, 0);
        for (int i = 0; i < trimmed.Length; i++)
        {
            Assert.AreEqual(full.Masses[fullIndex + i], trimmed.Masses[i], 1e-7);
            Assert.AreEqual(full.Intensities[fullIndex + i], trimmed.Intensities[i], 1e-12);
        }
    }

    [TestCase("Se2")]
    [TestCase("Fe2")]
    [TestCase("Xe2")]
    [TestCase("C213H320N66O65S2Se")]
    [TestCase("C210H315N65O64S2")]
    public void NegligiblePruningKeepsCentroidsAndIntensities(string formulaText)
    {
        var formula = ChemicalFormula.ParseString(formulaText.AsSpan(), _provider);
        var full = new Mercury7(0).GenerateIsotopicDistribution(formula);
        var pruned = new Mercury7(1e-26).GenerateIsotopicDistribution(formula);

        int fullIndex = 0;
        int prunedIndex = 0;
        while (fullIndex < full.Length && prunedIndex < pruned.Length)
        {
            if (Math.Abs(full.Masses[fullIndex] - pruned.Masses[prunedIndex]) < 0.5)
            {
                if (full.Intensities[fullIndex] >= 1e-8)
                {
                    Assert.AreEqual(full.Masses[fullIndex], pruned.Masses[prunedIndex], 1e-6);
                    Assert.AreEqual(full.Intensities[fullIndex], pruned.Intensities[prunedIndex], 1e-10);
                }
                fullIndex++;
                prunedIndex++;
            }
            else if (full.Masses[fullIndex] < pruned.Masses[prunedIndex])
            {
                Assert.Less(full.Intensities[fullIndex], 1e-26, $"Unexpectedly pruned interior peak for {formulaText}");
                fullIndex++;
            }
            else
            {
                Assert.Fail($"Pruning introduced a new mass for {formulaText}");
            }
        }

        while (fullIndex < full.Length)
        {
            Assert.Less(full.Intensities[fullIndex++], 1e-26, $"Unexpectedly pruned interior peak for {formulaText}");
        }
        Assert.AreEqual(pruned.Length, prunedIndex);
    }

    private void AssertMatchesOracle(ChemicalFormula formula)
    {
        var expected = AtomByAtomOracle(formula);
        var actual = new Mercury7(0).GenerateIsotopicDistribution(formula);
        Assert.AreEqual(expected.Count, actual.Length);
        for (int i = 0; i < expected.Count; i++)
        {
            Assert.AreEqual(expected[i].Mass, actual.Masses[i], 1e-10);
            Assert.AreEqual(expected[i].Abundance, actual.Intensities[i], 1e-12);
            Assert.Greater(actual.Masses[i], 0);
        }
    }

    // This deliberately expands one atom at a time and aggregates by total neutron
    // count. It does not use Mercury7's binary exponentiation or convolution routine.
    private static List<(double Mass, double Abundance)> AtomByAtomOracle(ChemicalFormula formula)
    {
        var states = new SortedDictionary<int, (double Probability, double WeightedMass)> { [0] = (1, 0) };
        foreach (var cardinality in formula.GetElements())
        {
            for (int atom = 0; atom < cardinality.Count; atom++)
            {
                var next = new SortedDictionary<int, (double Probability, double WeightedMass)>();
                foreach (var state in states)
                foreach (var isotope in cardinality.Entity.Isotopes)
                {
                    double contribution = state.Value.Probability * isotope.RelativeAbundance;
                    if (contribution == 0)
                        continue;
                    int neutronCount = state.Key + isotope.NeutronCount;
                    double weightedMass = state.Value.WeightedMass * isotope.RelativeAbundance
                        + contribution * isotope.AtomicMass;
                    if (next.TryGetValue(neutronCount, out var existing))
                        next[neutronCount] = (existing.Probability + contribution, existing.WeightedMass + weightedMass);
                    else
                        next[neutronCount] = (contribution, weightedMass);
                }
                states = next;
            }
        }

        return states.Values.Select(x => (x.WeightedMass / x.Probability, x.Probability)).ToList();
    }

    private sealed class MercuryTestElementProvider : InMemoryElementProvider
    {
        public MercuryTestElementProvider() : base(BuildElements()) { }

        private static IElement[] BuildElements()
        {
            var byAtomicNumber = new Dictionary<int, IElement>();
            foreach (var element in new MockElementProvider().GetElements())
                byAtomicNumber[element.AtomicNumber] = Reorder(element);

            Add(byAtomicNumber, 26, "Fe", (53.939609, 28, 0.05845), (55.934936, 30, 0.91754), (56.935393, 31, 0.02119), (57.933274, 32, 0.00282));
            Add(byAtomicNumber, 30, "Zn", (63.929142, 34, 0.4917), (65.926034, 36, 0.2773), (66.927128, 37, 0.0404), (67.924845, 38, 0.1845), (69.92532, 40, 0.0061));
            Add(byAtomicNumber, 35, "Br", (78.918338, 44, 0.5069), (80.916291, 46, 0.4931));
            Add(byAtomicNumber, 54, "Xe", (123.905893, 70, 0.00095), (125.904274, 72, 0.00089), (127.903531, 74, 0.0191), (128.904780, 75, 0.264), (129.903509, 76, 0.0407), (130.905084, 77, 0.212), (131.904155, 78, 0.269), (133.905394, 80, 0.104), (135.907220, 82, 0.0887));
            return byAtomicNumber.Values.ToArray();
        }

        private static IElement Reorder(IElement element) => new Element(element.AtomicNumber, element.Symbol,
            element.Isotopes.Reverse().ToArray());

        private static void Add(Dictionary<int, IElement> elements, int number, string symbol,
            params (double Mass, int Neutrons, double Abundance)[] isotopes)
        {
            elements[number] = new Element(number, symbol,
                isotopes.AsEnumerable().Reverse().Select(x => (IIsotope)new Isotope(x.Mass, x.Neutrons, x.Abundance)).ToArray());
        }
    }
}
