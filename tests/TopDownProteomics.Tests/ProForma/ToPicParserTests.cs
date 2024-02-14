using NUnit.Framework;
using System.IO;
using System.Text;
using TopDownProteomics.ProForma;

namespace TopDownProteomics.Tests.ProForma;

/// <summary>
/// Tests for the TopPicProformaParser <see cref="TopPicProformaParser"/>
/// </summary>
[TestFixture]
public class ToPicParserTests
{
    private static string GetTestDataFile(string name) => Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", name);

    /// <summary>
    /// Tests the TopPic Proforma Parser.
    /// </summary>
    [Test]
    [TestCase("M.A(AAA)[Phospho]AAA.C", "A(AAA)[UNIMOD:21|Info:Phospho]AAA")]
    [TestCase("W.(G)[Oxidation]DGCAQKNKPGVYTK(V)[Phospho]YNYVKWIKNTIAANS.", "[UNIMOD:35|Info:Oxidation]-GDGCAQKNKPGVYTKV[UNIMOD:21|Info:Phospho]YNYVKWIKNTIAANS")]
    [TestCase(".GDGCAQKNKPGVYTK(V)[Phospho]YNYVKWIKNTIAANS.", "GDGCAQKNKPGVYTKV[UNIMOD:21|Info:Phospho]YNYVKWIKNTIAANS")]
    [TestCase("W.GDGCAQKNKPGVYTKVYNYVKWIKNTIAAN(S)[Phospho].", "GDGCAQKNKPGVYTKVYNYVKWIKNTIAANS-[UNIMOD:21|Info:Phospho]")]
    [TestCase("W.GDGCAQKNKPGVYTKVYNYVKWIKNTIAANS.", "GDGCAQKNKPGVYTKVYNYVKWIKNTIAANS")]
    [TestCase(".(G)[Test1]DGCAQKNKPGVYTKVYNYVKWIKNTIAANS.", "[+59.000000|Info:Test1]-GDGCAQKNKPGVYTKVYNYVKWIKNTIAANS")]
    [TestCase("W.(G)[T@s!1]DGCAQKNKPGVYTKVYNYVKWIKNTIAANS.", "[T@s!1]-GDGCAQKNKPGVYTKVYNYVKWIKNTIAANS")]
    [TestCase("W.(G)[Test_2]DGCAQKNKPGVYTKVYNYVKWIKNTIAANS.", "[+59.000000|Info:Test_2]-GDGCAQKNKPGVYTKVYNYVKWIKNTIAANS")]
    [TestCase(".(G)[Ox_plus1]DGCAQKNKPGVYTKVYNYVKWIKNTIAANS.", "[+17.123000|Info:Ox_plus1]-GDGCAQKNKPGVYTKVYNYVKWIKNTIAANS")]
    [TestCase(".(G)[+23.9987]DGCAQKNKPGVYTKVYNYVKWIKNTIAANS.", "[+23.9987]-GDGCAQKNKPGVYTKVYNYVKWIKNTIAANS")]
    [TestCase(".(G)[23.9987]DGCAQKNKPGVYTKVYNYVKWIKNTIAANS.", "[+23.9987]-GDGCAQKNKPGVYTKVYNYVKWIKNTIAANS")]
    [TestCase(".(G)[-23.9987]DGCAQKNKPGVYTKVYNYVKWIKNTIAANS.", "[-23.9987]-GDGCAQKNKPGVYTKVYNYVKWIKNTIAANS")]
    public void CompareToProForma(string topPIC, string proForma)
    {
        var topicParser = new TopPicProformaParser(GetTestDataFile("topPicTestMods.txt"));
        var term = topicParser.ParseTopPicString(topPIC);

        var writer = new ProFormaWriter();

        Assert.AreEqual(proForma, writer.WriteString(term));
    }

    /// <summary>
    /// Tests the TopPic Proforma Parser with no mod file.
    /// </summary>
    [Test]
    [TestCase("M.A(AAA)[Phospho]AAA.C", "A(AAA)[Phospho]AAA")]
    [TestCase("W.(G)[Oxidation]DGCAQKNKPGVYTK(V)[Phospho]YNYVKWIKNTIAANS.", "[Oxidation]-GDGCAQKNKPGVYTKV[Phospho]YNYVKWIKNTIAANS")]
    [TestCase("W.(G)[asdf4fdfsd6!]DGCAQKNKPGVYTKYNYVKWIKNTIAANS.", "[asdf4fdfsd6!]-GDGCAQKNKPGVYTKYNYVKWIKNTIAANS")]
    public void CompareToProFormaNoModFile(string topPIC, string proForma)
    {
        var topicParser = new TopPicProformaParser();
        var term = topicParser.ParseTopPicString(topPIC);

        var writer = new ProFormaWriter();

        Assert.AreEqual(proForma, writer.WriteString(term));
    }

    /// <summary>
    /// Testing Exceptions.
    /// </summary>
    /// <param name="topPIC">The top pic.</param>
    [Test]
    [TestCase("M.A(AAA)[Phospho*4]AAA.C", "multiple mods are not currently supported")]
    [TestCase("M.A(AAA)[Phospho][Phospho]AAA.C", "multiple mods are not currently supported")]
    public void ParsingExceptionTesting(string topPIC, string exMessage)
    {
        var topicParser = new TopPicProformaParser(GetTestDataFile("topPicTestMods.txt"));

        TestDelegate throwTest = () =>
        {
            var term = topicParser.ParseTopPicString(topPIC);
        };

        TopPicParserException ex = Assert.Throws<TopPicParserException>(throwTest);
        Assert.AreEqual(exMessage, ex.Message);
    }

    /// <summary>
    /// Testing Exceptions.
    /// </summary>
    /// <param name="topPIC">The top pic.</param>
    [Test]
    [TestCase(@"Phospho,79.966331,STY,any,21,54", "Failed to parse mod file")]
    [TestCase(@"Phospho,79.966331,STY,any,2O", "Failed to parse UniMod Id 2O")]
    [TestCase(@"Phospho,79b.966331,STY,any,-1", "invalid UniMod Id or mass")]
    [TestCase(@"Phospho,79.966331,STY,any,-5", "invalid UniMod Id or mass")]
    public void ModFilePArsingExceptionTesting(string modFileString, string exMessage)
    {
        MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(modFileString));

        TestDelegate throwTest = () =>
        {     
            var topicParser = new TopPicProformaParser(stream);
        };

        TopPicParserException ex = Assert.Throws<TopPicParserException>(throwTest);
        Assert.AreEqual(exMessage, ex.Message);
    }
}