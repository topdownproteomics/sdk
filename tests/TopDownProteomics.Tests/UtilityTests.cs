using NUnit.Framework;

namespace TopDownProteomics.Tests;

[TestFixture]
public class UtilityTests
{
    [Test]
    public void OrderPairTest()
    {
        Assert.AreEqual((1, 2), Utility.OrderPair(1, 2));
        Assert.AreEqual((1, 2), Utility.OrderPair(2, 1));
        Assert.AreEqual((1, 1), Utility.OrderPair(1, 1));

        // Same tests with doubles
        Assert.AreEqual((1.0, 2.0), Utility.OrderPair(1.0, 2.0));
        Assert.AreEqual((1.0, 2.0), Utility.OrderPair(2.0, 1.0));
        Assert.AreEqual((1.0, 1.0), Utility.OrderPair(1.0, 1.0));
    }

    [Test]
    public void ConverterTest()
    {
        var mass = 8559.62;
        var charge = -6;
        var monoMz = 1425.596;

        var mz = Utility.ConvertMassToMz(mass, charge);

        // By convention, m/z is always positive even with negative charge
        Assert.AreEqual(monoMz, mz, 0.001);

        var mass2 = Utility.ConvertMzToMass(mz, charge);

        // By convention, mass is always positive even with negative charge
        Assert.IsTrue(mass2 > 0);
        Assert.AreEqual(mass, mass2, 0.0001);
    }

    [Test]
    public void ChargeCarrierTest()
    {
        var mass = 1000;
        var charge = 2;
        var chargeCarrier = 3d;

        var mz = Utility.ConvertMassToMz(mass, charge, chargeCarrier);

        // Should be 1000 / 2 + 3 = 503
        Assert.AreEqual(503, mz, 0.001);

        // Test round trip
        double mass2 = Utility.ConvertMzToMass(mz, charge, chargeCarrier);
        Assert.AreEqual(mass, mass2, 0.0001);

        // Let's try with a negative charge
        mz = Utility.ConvertMassToMz(mass, -charge, chargeCarrier);

        // Should be 1000 / 2 - 3 = 497
        Assert.AreEqual(497, mz, 0.001);
    }
}
