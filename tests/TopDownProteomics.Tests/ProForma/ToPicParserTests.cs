using NUnit.Framework;
using System.Linq;
using TopDownProteomics.ProForma;

namespace TopDownProteomics.Tests.ProForma
{
    /// <summary>
    /// Tests for the TopPicProformaParser <see cref="TopPicProformaParser"/>
    /// </summary>
    [TestFixture]
    public class ToPicParserTests
    {
        /// <summary>
        /// Tests the TopPic Proforma Parser.
        /// </summary>
        [Test]
        [TestCase("W.(G)[Oxidation]DGCAQKNKPGVYTK(V)[Phospho]YNYVKWIKNTIAANS.", "UNIMOD:35", null, 1, "UNIMOD:21", 15)]
        [TestCase("W.GDGCAQKNKPGVYTK(V)[Phospho]YNYVKWIKNTIAANS.", null, null, 1, "UNIMOD:21", 15)]
        [TestCase("W.GDGCAQKNKPGVYTKVYNYVKWIKNTIAAN(S)[Phospho].", null, "UNIMOD:21", 0, null, null)]
        [TestCase("W.GDGCAQKNKPGVYTKVYNYVKWIKNTIAANS.", null, null, 0, null, null)]
        [TestCase("W.(G)[Test1]DGCAQKNKPGVYTKVYNYVKWIKNTIAANS.", "Test1", null, 0, null, null)]
        [TestCase("W.(G)[Test_2]DGCAQKNKPGVYTKVYNYVKWIKNTIAANS.", "Test_2", null, 0, null, null)]
        [TestCase("W.(G)[Ox_plus1]DGCAQKNKPGVYTKVYNYVKWIKNTIAANS.", "Ox_plus1", null, 0, null, null)]
        [TestCase("W.(G)[+23.9987]DGCAQKNKPGVYTKVYNYVKWIKNTIAANS.", "+23.9987", null, 0, null, null)]
        public void TestParser(string topPicString, string? nTermModAccession, string? cTermModAccession, int tagCount, string? firstTagAccession, int? firstTagIndex)
        {
            var topicParser = new TopPicProformaParser(@".\TestData\topPicTestMods.txt");
            var term =  topicParser.ParseTopPicString(topPicString);

            Assert.IsNotNull(term);

            //Test NTerm
            if (term.NTerminalDescriptors.Any())
                Assert.AreEqual(nTermModAccession, term.NTerminalDescriptors.First().Value);
            else
                Assert.IsNull(nTermModAccession);

            //Test Cterm
            if (term.CTerminalDescriptors.Any())
                Assert.AreEqual(cTermModAccession, term.CTerminalDescriptors.First().Value);
            else
                Assert.IsNull(cTermModAccession);

            //Test Tags
            Assert.AreEqual(term.Tags.Count, tagCount);

            if (term.Tags.Any())
            {
                Assert.AreEqual(firstTagAccession, term.Tags.First().Descriptors.First().Value);
                Assert.AreEqual(firstTagIndex, term.Tags.First().ZeroBasedStartIndex);
            }
            else
            {
                Assert.IsNull(firstTagAccession);
                Assert.IsNull (firstTagIndex);
            }
        }
    }
}