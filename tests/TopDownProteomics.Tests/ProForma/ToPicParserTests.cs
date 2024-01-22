﻿using NUnit.Framework;
using System.Linq;
using TopDownProteomics.ProForma;

namespace TopDownProteomics.Tests.ProForma;

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

    public void CompareToProForma(string topPIC, string proForma)
    {
        var topicParser = new TopPicProformaParser(@".\TestData\topPicTestMods.txt");
        var term = topicParser.ParseTopPicString(topPIC);

        var writer = new ProFormaWriter();

        Assert.AreEqual(proForma, writer.WriteString(term));
    }
}