using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TopDownProteomics.ProForma;
using TopDownProteomics.Proteomics;

namespace TopDownProteomics.Tests
{
    [TestFixture]
    public static class ProteoformClassifierTest
    {
        [Test]
        [TestCase("EMEVEESPEK", 1, "1")]
        [TestCase("EM[UNIMOD:35]EVEES[UNIMOD:21]PEK", 1, "1")]
        [TestCase("[+42]-EMEVEES[UNIMOD:21]PEK", 1, "2B")]
        [TestCase("EM[UNIMOD:35]EVEESPEK-[+42]", 1, "2B")]
        [TestCase("{Phospho}EM[UNIMOD:35]EVEESPEK", 1, "2A")]
        [TestCase("{+80}EM[UNIMOD:35]EVEESPEK", 1, "3")]
        [TestCase("SEQUEN[Formula:C12H20O2]CE", 1, "1")]
        [TestCase("[Phospho]?EMEVTSESPEK", 1, "2A")]
        [TestCase("EMEVT[#g1]S[#g1]ES[Phospho#g1]PEK", 1, "2A")]
        [TestCase("EM[+15.9949]EVEES[-79.9663]PEK", 1, "2B")]
        [TestCase("EMEVEESPEK[+42|Info:likely Acetyl or Trimethyl]", 1, "2B")]
        [TestCase("EMEVEE(?SP)EK", 1, "2C")]
        [TestCase("PROTEOSFORMSISK(?N)", 1, "2C")]
        [TestCase("EMEVEESPEKB", 1, "2C")]
        [TestCase("EMEVEESPEKJ", 1, "2C")]
        [TestCase("EMEVEESPEKX", 1, "2C")]
        [TestCase("EMEVEESPEKZ", 1, "2C")]
        [TestCase("EMEVEESPEK", 2, "2D")]
        [TestCase("PROT(EOSFORMS)[+19.0523]ISKN", 1, "3")]
        [TestCase("PROT(EOSFORMS)[+19.0523]ISK(?N)", 1, "4")]
        [TestCase("PROT(EOSFORMS)[+19.0523]ISK(?N)", 2, "5")]
        [TestCase("PROT(?EOSFORMS)[Oxidation]ISKN", 2, "4")]
        [TestCase("PROT(?EOSFORMS)[+19.0523]ISKN", 2, "5")]
        [TestCase("PROT(?EOSFORMS[+19.0523])ISKN", 2, "5")]
        [TestCase("PROT(?EOSFORMS)IS[+19.0523]KN", 2, "4")]
        [TestCase("PROT(?EOSFORMS)IS(KK)[Acetyl]", 1, "3")]
        public static void TestProForma_ProteoformClassification(string proFormaString, int numGenes, string expectedLevel, bool checkWriter = true)
        {
            List<string> genes = Enumerable.Range(0, numGenes).Select(x => x.ToString()).ToList();

            //parse string
            ProFormaParser parser = new();
            ProFormaTerm parsedProteoform = parser.ParseString(proFormaString);

            //check that the level is what we expect
            string level = FiveLevelProteoformClassifier.ClassifyProForma(parsedProteoform, genes);
            Assert.AreEqual(expectedLevel, level);

            //check that we can write what we read
            if (checkWriter)
            {
                ProFormaWriter writer = new();
                string writtenProForma = writer.WriteString(parsedProteoform);
                Assert.AreEqual(proFormaString, writtenProForma);
            }
        }
    }
}