using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TopDownProteomics.IO.Xlmod;

namespace TopDownProteomics.Tests.IO
{
    [TestFixture]
    public class XlmodOboParserTest
    {
        public static string GetFilePath() => Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "XLMOD.obo");

        [Test]
        public void BasicTest()
        {
            var parser = new XlmodOboParser();

            var result = new List<XlmodTerm>(parser.Parse(GetFilePath()));

            Assert.IsNotNull(result);
            Assert.AreEqual(1102, result.Count);
            Assert.AreEqual("XLMOD:00004", result[4].Id);
            Assert.AreEqual("cross-linking entity", result[1].Name);
            Assert.AreEqual("Dead-end modification resulting from a cross-linker reagent reacting only with one peptide.", result[2].Definition);

            Assert.IsNull(result[0].IsA);
            Assert.IsNull(result[0].PropertyValues);
            Assert.IsNull(result[0].Relationships);
            Assert.IsNull(result[0].Synonyms);

            // External References
            Assert.IsNotNull(result[166].ExternalReferences);
            Assert.AreEqual(1, result[166].ExternalReferences.Count);
            Assert.AreEqual("21557289", result[166].ExternalReferences.Single(x => x.Name == "PMID").Id);
            var term7091 = result.Single(x => x.Id == "XLMOD:07091");
            Assert.IsNotNull(term7091.ExternalReferences);
            Assert.AreEqual(4, term7091.ExternalReferences.Count);
            Assert.AreEqual("18704-37-5", term7091.ExternalReferences.Single(x => x.Name == "CAS").Id);
            Assert.AreEqual("29220", term7091.ExternalReferences.Single(x => x.Name == "PubChem_Compound").Id);
            Assert.AreEqual("27177", term7091.ExternalReferences.Single(x => x.Name == "ChemSpiderID").Id);
            Assert.AreEqual("CB8329163", term7091.ExternalReferences.Single(x => x.Name == "ChemicalBookNo").Id);

            // Is A
            Assert.IsNotNull(result[4].IsA);
            Assert.AreEqual(1, result[4].IsA.Count);

            // Property values
            Assert.IsNotNull(result[101].PropertyValues);
            Assert.AreEqual(2, result[101].PropertyValues.Count);
            Assert.AreEqual("(K,Protein N-term)", result[101].PropertyValues.Single(x => x.Name == "specificities").Value);
            Assert.AreEqual("xsd:string", result[101].PropertyValues.Single(x => x.Name == "secondarySpecificities").DataType);

            // Relationships
            Assert.IsNotNull(result[103].Relationships);
            Assert.AreEqual(2, result[103].Relationships.Count);
            Assert.AreEqual("XLMOD:00149", result[103].Relationships.Single(x => x.Type == "is_activatable").Id);
            Assert.AreEqual("XLMOD:00037", result[103].Relationships.Single(x => x.Type == "is_reactive_with").Id);

            // Synonyms
            Assert.IsNotNull(result[102].Synonyms);
            Assert.AreEqual(1, result[102].Synonyms.Count);
            Assert.AreEqual("N-hydroxysulfosuccinimide ester", result[102].Synonyms.Single().Text);
            Assert.AreEqual("EXACT", result[102].Synonyms.Single().Type);
            var term2001 = result.Single(x => x.Id == "XLMOD:02001");
            Assert.AreEqual("EXACT", term2001.Synonyms.Single(x=>x.Text == "1,1'-[(1,8-Dioxooctane-1,8-diyl)bis(oxy)]dipyrrolidine-2,5-dione").Type);
        }
    }
}