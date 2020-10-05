using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using TopDownProteomics.IO.Unimod;

namespace TopDownProteomics.Tests.IO
{
    [TestFixture]
    public class UnimodTest
    {
        public static string GetUnimodFilePath() => Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "unimod.obo");

        [Test]
        public void BasicTest()
        {
            var parser = new UnimodOboParser();

            var mods = parser.Parse(GetUnimodFilePath()).ToList();
            Assert.AreEqual(2017, mods.Max(x => Convert.ToInt32(x.Id.Substring(7))));

            UnimodModification unimod1 = mods.Single(x => x.Id == "UNIMOD:1");

            Assert.IsNotNull(unimod1);
            Assert.AreEqual("Acetyl", unimod1.Name);
            Assert.AreEqual(@"""Acetylation."" [RESID:AA0048, RESID:AA0049, RESID:AA0041, RESID:AA0052, RESID:AA0364, RESID:AA0056, RESID:AA0046, RESID:AA0051, RESID:AA0045, RESID:AA0354, RESID:AA0044, RESID:AA0043, PMID:11999733, URL:http://www.ionsource.com/Card/acetylation/acetylation.htm, RESID:AA0055, PMID:14730666, PMID:15350136, RESID:AA0047, PMID:12175151, PMID:11857757, RESID:AA0042, RESID:AA0050, RESID:AA0053, RESID:AA0054, FindMod:ACET, UNIMODURL:http://www.unimod.org/modifications_view.php?editid1=1]", unimod1.Definition);
            Assert.AreEqual("H(2) C(2) O", unimod1.DeltaComposition);
            Assert.AreEqual(42.0367, unimod1.DeltaAverageMass);
            Assert.AreEqual(42.010565, unimod1.DeltaMonoisotopicMass);
        }
    }
}