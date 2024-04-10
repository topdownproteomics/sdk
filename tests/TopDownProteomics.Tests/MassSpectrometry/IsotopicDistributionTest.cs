using NUnit.Framework;
using System;
using System.Collections;
using TopDownProteomics.Chemistry;
using TopDownProteomics.MassSpectrometry;
using TopDownProteomics.Tools;

namespace TopDownProteomics.Tests.MassSpectrometry;

[TestFixture]
public class IsotopicDistributionTest
{
    private readonly IElementProvider _elementProvider = new MockElementProvider();

    [Test]
    public void MercuryTest()
    {
        var mercury = new Mercury7(1e-30);

        IChargedIsotopicDistribution result = mercury.GenerateChargedIsotopicDistribution(ChemicalFormula.ParseString("C6H12O6".AsSpan(), _elementProvider), 1);

        var mz = result.GetMz();
        var abun = result.GetIntensity();

        Assert.AreEqual(181.07066, mz[0], 0.0001);
        Assert.AreEqual(0.9226, abun[0], 0.0001);

        IIsotopicDistribution dist = mercury.GenerateIsotopicDistribution(ChemicalFormula.ParseString("C6H12O6".AsSpan(), _elementProvider));
        IChargedIsotopicDistribution charge1 = dist.CreateChargedDistribution(1);

        CollectionAssert.AreEquivalent((ICollection)mz, (ICollection)charge1.GetMz());
    }

    [Test]
    public void MercuryNegTest()
    {
        var mercury = new Mercury7(1e-30);

        ChemicalFormula formula = ChemicalFormula.ParseString("C6H12O6".AsSpan(), _elementProvider);
        IChargedIsotopicDistribution result = mercury.GenerateChargedIsotopicDistribution(formula, -1);

        var mz = result.GetMz();
        var abun = result.GetIntensity();

        Assert.IsTrue(mz.Length == abun.Length);
        Assert.AreEqual(formula.GetMass(MassType.Monoisotopic) - 1.007276466, mz[0], 0.001);
    }

    [Test]
    public void MercuryChargeCarrierTest()
    {
        var chargeCarrier = 3d;
        var mercury = new Mercury7(1e-30, chargeCarrier);

        ChemicalFormula formula = ChemicalFormula.ParseString("C6H12O6".AsSpan(), _elementProvider);
        IChargedIsotopicDistribution result = mercury.GenerateChargedIsotopicDistribution(formula, 1);

        Assert.AreEqual(formula.GetMass(MassType.Monoisotopic) + chargeCarrier, result.FirstMz, 0.001);

        // Test negative charge
        result = mercury.GenerateChargedIsotopicDistribution(formula, -1);

        Assert.AreEqual(formula.GetMass(MassType.Monoisotopic) - chargeCarrier, result.FirstMz, 0.001);
    }

    [Test]
    public void MercuryResultCloneTest()
    {
        double[] mz = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
        double[] intensity = { 1.1, 2.1, 3.1, 4.2, 5.1, 6.1, 5.2, 4.1, 3.1, 2.1, 1.1 };

        ChargedIsotopicDistribution result = new ChargedIsotopicDistribution(mz, intensity, 1);
        IChargedIsotopicDistribution clone = result.CloneWithMostIntensePoints(1);

        double[] cloneMz = clone.GetMz();
        double[] cloneIntensity = clone.GetIntensity();

        Assert.AreEqual(1, clone.Length);
        Assert.AreEqual(6, cloneMz[0]);
        Assert.AreEqual(6.1, cloneIntensity[0]);

        clone = result.CloneWithMostIntensePoints(2);
        cloneMz = clone.GetMz();
        cloneIntensity = clone.GetIntensity();

        Assert.AreEqual(2, clone.Length);
        Assert.AreEqual(7, cloneMz[1]);
        Assert.AreEqual(5.2, cloneIntensity[1]);

        clone = result.CloneWithMostIntensePoints(4);
        cloneMz = clone.GetMz();
        cloneIntensity = clone.GetIntensity();

        Assert.AreEqual(4, clone.Length);
        Assert.AreEqual(4, cloneMz[0]);
        Assert.AreEqual(4.2, cloneIntensity[0]);

        clone = result.CloneWithMostIntensePoints(7);
        cloneMz = clone.GetMz();
        cloneIntensity = clone.GetIntensity();

        Assert.AreEqual(7, clone.Length);
        Assert.AreEqual(8, cloneMz[5]);
        Assert.AreEqual(5.1, cloneIntensity[2]);
    }

    [Test]
    public void MercuryTrimTest()
    {
        var mercury = new Mercury7(1e-30);
        IIsotopicDistribution original = mercury.GenerateIsotopicDistribution(ChemicalFormula.ParseString("C6H12O6".AsSpan(), _elementProvider));

        mercury = new Mercury7(1e-5);
        IIsotopicDistribution trimmed = mercury.GenerateIsotopicDistribution(ChemicalFormula.ParseString("C6H12O6".AsSpan(), _elementProvider));

        Assert.AreEqual(original.Masses[0], trimmed.Masses[0], 1e-5);
    }

    [Test]
    public void HighPruningLimit()
    {
        ChemicalFormula chemicalFormula = ChemicalFormula.ParseString("C6H12O6".AsSpan(), _elementProvider);

        var calc = new Mercury7(1);
        IChargedIsotopicDistribution result = calc.GenerateChargedIsotopicDistribution(chemicalFormula, 1);
        Assert.AreEqual(0, result.Length);

        calc = new Mercury7(0.1);
        result = calc.GenerateChargedIsotopicDistribution(chemicalFormula, 1);
        Assert.AreEqual(1, result.Length);

        calc = new Mercury7(0.005);
        result = calc.GenerateChargedIsotopicDistribution(chemicalFormula, 1);
        Assert.AreEqual(2, result.Length);

        calc = new Mercury7(0.001);
        result = calc.GenerateChargedIsotopicDistribution(chemicalFormula, 1);
        Assert.AreEqual(3, result.Length);
    }
}