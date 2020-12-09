using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TopDownProteomics.IO.PsiMod;
using TopDownProteomics.Proteomics;

namespace TopDownProteomics.Tests.IO
{
    [TestFixture]
    public class PsiModOboParserTest
    {
        public static string GetFilePath() => Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "PSI-MOD.obo");

        [Test]
        public void BasicTest()
        {
            var parser = new PsiModOboParser();

            List<PsiModTerm> result = new List<PsiModTerm>(parser.Parse(GetFilePath()));

            Assert.IsNotNull(result);
            Assert.AreEqual(2027, result.Count);
            Assert.AreEqual("MOD:00812", result[812].Id);
            Assert.AreEqual("alkylated residue", result[1].Name);
            Assert.AreEqual("A protein modification that effectively converts an L-serine residue to O3-glycosylserine.", result[2].Definition);
            Assert.AreEqual("This term is for organizational use only and should not be assigned. [JSG]", result[3].Comment);

            Assert.AreEqual(146.14, result[813].DiffAvg.Value, 0.001);
            Assert.IsNull(result[434].DiffAvg); // Handle 'none'

            Assert.AreEqual("C 6 H 10 N 0 O 4", result[813].DiffFormula);
            Assert.AreEqual(146.057909, result[813].DiffMono.Value, 0.001);

            Assert.AreEqual(247.25, result[813].MassAvg.Value, 0.001);
            Assert.AreEqual("C 10 H 17 N 1 O 6", result[813].Formula);
            Assert.AreEqual(247.105587, result[813].MassMono.Value, 0.001);

            Assert.AreEqual(true, result[4].IsObsolete);
            Assert.AreEqual(false, result[813].IsObsolete);

            // Should be just one character ... if more, report nothing
            // TODO: Handle cross-links differently
            Assert.AreEqual('T', result[813].Origin);
            Assert.AreEqual(null, result[34].Origin); // Cross-link, origin is C, C
            Assert.AreEqual(null, result[1038].Origin); // says 'none'
            Assert.AreEqual(null, result[458].Origin); // Special case for 'X'

            Assert.AreEqual(null, result[0].Source);
            Assert.AreEqual(null, result[1].Source);
            Assert.AreEqual(PsiModModificationSource.Natural, result[812].Source);
            Assert.AreEqual(PsiModModificationSource.Artifact, result[7].Source);
            Assert.AreEqual(PsiModModificationSource.Hypothetical, result[231].Source);

            Assert.AreEqual(null, result[813].Terminus); // says 'none'
            Assert.AreEqual(null, result[9].Terminus); // doesn't exist
            Assert.AreEqual(Terminus.N, result[30].Terminus);
            Assert.AreEqual(Terminus.C, result[90].Terminus);

            Assert.IsNull(result[4].IsA);
            Assert.AreEqual(2, result[5].IsA.Count);
            CollectionAssert.Contains(result[5].IsA.ToList(), "MOD:00396");
            CollectionAssert.Contains(result[5].IsA.ToList(), "MOD:00917");

            PsiModTerm formylMethionine = result[30];
            Assert.AreEqual(9, formylMethionine.ExternalReferences.Count);
            CollectionAssert.Contains(formylMethionine.ExternalReferences.Select(x => x.Id).ToList(), "AA0021#FMET");
            CollectionAssert.Contains(formylMethionine.ExternalReferences.Select(x => x.Name).ToList(), "RESID");

            Assert.AreEqual(11, formylMethionine.Synonyms.Count);
            CollectionAssert.Contains(formylMethionine.Synonyms.Select(x => x.Scope).ToList(), "EXACT");
            CollectionAssert.Contains(formylMethionine.Synonyms.Select(x => x.Text).ToList(), "2-formylamino-4-(methylthio)butanoic acid");
            CollectionAssert.Contains(formylMethionine.Synonyms.Select(x => x.Type).ToList(), "PSI-MOD-alternate");

            // Check formal charge
            Assert.AreEqual("MOD:00083", result[83].Id);
            PsiModTerm trimethylLysine = result[83];
            Assert.AreEqual(1, trimethylLysine.FormalCharge);

            PsiModTerm dimethylLysine = result[84];
            Assert.AreEqual(0, dimethylLysine.FormalCharge);

            PsiModTerm hexakis = result[147];
            Assert.AreEqual(-3, hexakis.FormalCharge);

            PsiModTerm stearoylated = result[2001];
            Assert.AreEqual("MOD:02001", stearoylated.Id);
            Assert.IsNull(stearoylated.ExternalReferences);
            Assert.IsNull(stearoylated.Remap);

            // Check for Obsolete and Remap
            PsiModTerm residueMethylEster = result[407];
            Assert.IsTrue(residueMethylEster.IsObsolete);
            Assert.AreEqual("MOD:00599", residueMethylEster.Remap);
        }
    }
}