using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TopDownProteomics.IO.Resid;
using TopDownProteomics.Proteomics;

namespace TopDownProteomics.Tests.IO
{
    [TestFixture]
    public class ResidXmlParserTest
    {
        public static string GetResidFilePath() => Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "RESIDUES.xml");

        [Test]
        public void BasicTest()
        {
            var parser = new ResidXmlParser();
            List<ResidModification> modifications = parser.Parse(GetResidFilePath()).ToList();

            ResidModification r42 = modifications.Single(x => x.Id == 42);
            Assert.IsNotNull(r42);
            Assert.AreEqual("N-acetyl-L-aspartic acid", r42.Name);
            Assert.AreEqual("C 6 H 8 N 1 O 4", r42.Formula);
            Assert.AreEqual(158.13, r42.AverageMass);
            Assert.AreEqual(158.045333, r42.MonoisotopicMass);
            Assert.AreEqual("C 2 H 2 N 0 O 1", r42.DiffFormula);
            Assert.AreEqual(42.04, r42.DiffAverageMass);
            Assert.AreEqual(42.010565, r42.DiffMonoisotopicMass);
            Assert.AreEqual('D', r42.Origin);
            Assert.AreEqual(Terminus.N, r42.Terminus);
            Assert.AreEqual("N-acetylaspartate", r42.SwissprotTerm);
        }

        [Test]
        public void TwoCorrectionBlocksTest()
        {
            ResidXmlParser parser = new ResidXmlParser();
            List<ResidModification> modifications = parser.Parse(GetResidFilePath()).ToList();

            ResidModification r1 = modifications.Single(x => x.Id == 1);
            Assert.IsFalse(r1.DiffAverageMass.HasValue);

            ResidModification r21 = modifications.Single(x => x.Id == 21);
            Assert.IsTrue(r21.DiffAverageMass.HasValue);
            Assert.AreEqual(r21.DiffAverageMass.Value, 28.01, 0.0001);

            ResidModification r42 = modifications.Single(x => x.Id == 42);
            Assert.IsTrue(r42.DiffMonoisotopicMass.HasValue);
            Assert.AreEqual(r42.DiffMonoisotopicMass.Value, 42.010565, 0.0001);
        }

        [Test]
        public void FormalChargeTest()
        {
            ResidXmlParser parser = new ResidXmlParser();
            List<ResidModification> modifications = parser.Parse(GetResidFilePath()).ToList();

            ResidModification r74 = modifications.Single(x => x.Id == 74);
            Assert.AreEqual(1, r74.FormalCharge);
            //Assert.AreEqual(42.04695, r74.GetDeltaMass(MassType.Monoisotopic), 0.0001);
        }
    }
}