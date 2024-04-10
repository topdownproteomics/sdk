using NUnit.Framework;
using System;
using TopDownProteomics.Chemistry;
using TopDownProteomics.Tools;

namespace TopDownProteomics.Tests.Tools;

[TestFixture]
public class FineStructureTest
{
    private readonly IElementProvider _elementProvider = new MockElementProvider();

    [Test]
    public void BasicTest()
    {
        var generator = new FineStructureIsotopicGenerator();

        var result = generator.GenerateIsotopicDistribution(ChemicalFormula.ParseString("C6H12O6".AsSpan(), _elementProvider));

        var mz = result.Masses;
        var abun = result.Intensities;

        Assert.AreEqual(180.0634, mz[0], 0.0001);
        Assert.AreEqual(0.9226, abun[0], 0.0001);
    }
}