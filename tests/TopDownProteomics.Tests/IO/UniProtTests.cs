using NUnit.Framework;
using System.Linq;
using TopDownProteomics.IO.UniProt;

namespace TopDownProteomics.Tests.IO
{
    [TestFixture]
    public class UniProtTests
    {
        [Test]
        public void BasicModificationFileParse()
        {
            var parser = new UniProtPtmListParser();
            var entries = parser.Parse(Get_PtmList()).ToList();

            Assert.AreEqual(3, entries.Count);

            UniprotModification mod1 = entries[1];
            Assert.AreEqual("(3R)-3-hydroxyasparagine", mod1.Identifier);
            Assert.AreEqual("PTM-0369", mod1.Accession);
            Assert.AreEqual(UniprotFeatureType.ModifiedResidue, mod1.FeatureKey);
            Assert.AreEqual("Asparagine", mod1.Target);
            Assert.AreEqual("Amino acid side chain", mod1.AminoAcidPosition);
            Assert.AreEqual("Anywhere", mod1.PolypeptidePosition);
            Assert.AreEqual("O1", mod1.CorrectionFormula);
            Assert.AreEqual(15.994915, mod1.MonoisotopicMassDifference);
            Assert.AreEqual(16.00, mod1.AverageMassDifference);
            Assert.AreEqual("Intracellular localisation or Extracellular and lumenal localisation", mod1.CellularLocation);

            Assert.AreEqual(1, mod1.TaxonomicRanges.Count);
            Assert.AreEqual("Eukaryota; taxId:40674 (Mammalia)", mod1.TaxonomicRanges[0]);
            Assert.AreEqual(1, mod1.Keywords.Count);
            Assert.AreEqual("Hydroxylation", mod1.Keywords[0]);

            Assert.AreEqual("AA0026", mod1.Resid);
            Assert.AreEqual("00035", mod1.PsiMod);
        }

        // Copied top 3 ptms from the file
        public static string Get_PtmList()
        {
            return @"----------------------------------------------------------------------------
        UniProt Knowledgebase:
          Swiss-Prot Protein Knowledgebase
          TrEMBL Protein Database
        Swiss Institute of Bioinformatics (SIB); Geneva, Switzerland
        European Bioinformatics Institute (EBI); Hinxton, United Kingdom
        Protein Information Resource (PIR); Washington DC, USA
----------------------------------------------------------------------------

Description: Controlled vocabulary of posttranslational modifications (PTM)
Name:        ptmlist.txt
Release:     2011_03 of 08-Mar-2011

----------------------------------------------------------------------------

  This document lists the posttranslational modifications used in the UniProt
  knowledgebase (Swiss-Prot and TrEMBL).

  The definition of the posttranslational modifications usage as well as other
  information is provided in the following format:

  ---------  ---------------------------     ----------------------
  Line code  Content                         Occurrence in an entry
  ---------  ---------------------------     ----------------------
  ID         Identifier (FT description)     Once; starts a PTM entry.
  AC         Accession (PTM-xxxx)            Once.
  FT         Feature key                     Once.
  TG         Target                          Once; two targets separated
                                             by a dash in case of intrachain
                                             crosslinks.
  PA         Position of the modification    Optional, once.
             on the amino acid
  PP         Position of the modification    Optional, once.
             in the polypeptide
  CF         Correction formula              Optional, once.
  MM         Monoisotopic mass difference    Optional, once.
  MA         Average mass difference         Optional, once.
  LC         Cellular location               Optional, once; alternatives
                                             can be proposed.
  TR         Taxonomic range                 Optional, once or more.
  KW         Keyword                         Optional, once or more.
  DR         Cross-reference to PTM          Optional, once or more.
             databases
  //         Terminator                      Once; ends an entry.


AN   Next free AC: PTM-0440
______________________________________________________________________________
ID   (2-aminosuccinimidyl)acetic acid (Asp-Gly)
AC   PTM-0312
FT   CROSSLNK
TG   Aspartate-Glycine.
PA   Amino acid side chain-Amino acid backbone.
PP   Anywhere-Protein core.
CF   H-2 O-1
MM   -18.010565
MA   -18.02
LC   Extracellular and lumenal localisation.
TR   Eukaryota; taxId:3674 (Momordica cochinchinensis).
DR   RESID; AA0441.
DR   PSI-MOD; 00952.
//
ID   (3R)-3-hydroxyasparagine
AC   PTM-0369
FT   MOD_RES
TG   Asparagine.
PA   Amino acid side chain.
PP   Anywhere.
CF   O1
MM   15.994915
MA   16.00
LC   Intracellular localisation or Extracellular and lumenal localisation.
TR   Eukaryota; taxId:40674 (Mammalia).
KW   Hydroxylation.
DR   RESID; AA0026.
DR   PSI-MOD; 00035.
//
ID   (3R)-3-hydroxyaspartate
AC   PTM-0371
FT   MOD_RES
TG   Aspartate.
PA   Amino acid side chain.
PP   Anywhere.
CF   O1
MM   15.994915
MA   16.00
LC   Extracellular and lumenal localisation.
TR   Bacteria; taxId:68215 (Streptoverticillium griseoverticillatum).
TR   Eukaryota; taxId:40674 (Mammalia).
KW   Hydroxylation.
DR   RESID; AA0027.
DR   PSI-MOD; 00036.
//";
        }
    }
}