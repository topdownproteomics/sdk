using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TopDownProteomics.IO.MzIdentMl;


#nullable enable

namespace TopDownProteomics.Tests.IO
{
	[TestFixture]
	public class MzIdentMlTest
	{
		public static string GetFilePath() => Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "MzIdentMl.mzid");
		internal static string GoldenTDPortalPath => Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "golden.mzid");

		[Test]
		public void MultipleSpectrumIdentificationProtocolsTest()
		{
			using (var parser = new MzIdentMlParser(GoldenTDPortalPath))
			{
				List<MzIdentMlAnalysisSoftware> software = parser.GetAnalysisSoftware().ToList();
				Assert.AreEqual(software.Count, 1);
				Assert.AreEqual(software[0].Name, "TDPortal");
				Assert.AreEqual(software[0].Version, "4.0.0");

				List<MzIdentMlDatabaseSequence> sequences = parser.GetDatabaseSequences().ToList();
				Assert.AreEqual(sequences.Count, 186);
				Assert.AreEqual(sequences[0].Accession, "Q9P0S9");

				List <MzIdentMlFragmentationMeasure> measures = parser.GetFragmentationMeasures().ToList();
				Assert.AreEqual(measures.Count, 2);
				Assert.AreEqual(measures[0].Measure.Accession, MzIdentMlParser.ProductIonMzCvAccession);
				Assert.AreEqual(measures[1].Measure.Accession, MzIdentMlParser.ProductIonMzErrorCvAccession);

				MzIdentMlInputs inputs = parser.GetInputs();
				Assert.AreEqual(inputs.SearchDatabases.Count, 1);
				Assert.AreEqual(inputs.SourceFiles.Count, 1);
				Assert.AreEqual(inputs.SpectraData.Location, @"/projects/b1035/scratch/staging_prod_github/10054/working//Rawfiles/golden.raw");

				List <MzIdentMlPeptideEvidence> evidences = parser.GetPeptideEvidences().ToList();
				Assert.AreEqual(evidences.Count, 346);
				Assert.AreEqual(evidences[0].Id, "PE_Chem_1730_ISO_1513");
				Assert.AreEqual(evidences[0].Peptide.Id, "Chem_1730");

				List <MzIdentMlPeptide> peptides = parser.GetPeptides().ToList();
				Assert.AreEqual(peptides.Count, 345);
				Assert.AreEqual(peptides[0].Id, "Chem_1730");
				Assert.AreEqual(peptides[0].PeptideEvidences.Count, 1);

				List<MzIdentMlProteinDetectionList> proteinDetectionLists = parser.GetProteinDetectionLists().ToList();
				Assert.AreEqual(proteinDetectionLists.Count, 2);
				Assert.AreEqual(proteinDetectionLists[0].Name, "BioMarker");
				Assert.AreEqual(proteinDetectionLists[0].ProteinAmbiguityGroups.Count, 121);
				Assert.AreEqual(proteinDetectionLists[1].ProteinAmbiguityGroups.Count, 169);

				List <MzIdentMlProteinDetectionProtocol> proteinDetectionProtocols = parser.GetProteinDetectionProtocols().ToList();
				Assert.AreEqual(proteinDetectionProtocols.Count, 2);
				Assert.AreEqual(proteinDetectionProtocols[0].AnalysisParams.UserParams.Count, 10);

				List<MzIdentMlSpectrumIdentificationItem> spectrumIdItems = parser.GetSpectrumIdentificationItems().ToList();
				Assert.AreEqual(spectrumIdItems.Count, 1461);
				Assert.AreEqual(spectrumIdItems[0].PeptideEvidences[0].Id, "PE_Chem_1730_ISO_1513");
				Assert.AreEqual(spectrumIdItems[0].Peptides[0].Id, "Chem_1730");

				List<MzIdentMlSpectrumIdentificationList> spectrumIdLists = parser.GetSpectrumIdentificationLists().ToList();
				Assert.AreEqual(spectrumIdLists.Count, 2);
				Assert.AreEqual(spectrumIdLists[0].Name, "BioMarker");
				Assert.AreEqual(spectrumIdLists[0].SpectrumIdentificationResults.Count, 401);
				Assert.AreEqual(spectrumIdLists[1].Name, "Tight Absolute Mass");
				Assert.AreEqual(spectrumIdLists[1].SpectrumIdentificationResults.Count, 571);


				Assert.AreEqual(spectrumIdLists[1].SpectrumIdentificationResults[0].SpectralData.Location, @"/projects/b1035/scratch/staging_prod_github/10054/working//Rawfiles/golden.raw");
				Assert.AreEqual(spectrumIdLists[1].SpectrumIdentificationResults[0].SpectrumScanNumber, 1426);
				Assert.AreEqual(spectrumIdLists[1].SpectrumIdentificationResults[4].Id, "SIR_2_1_1032");
				Assert.AreEqual(spectrumIdLists[1].SpectrumIdentificationResults[4].SpectrumIdentificationItems.Count, 2);

				List <MzIdentMlSpectrumIdentificationProtocol> spectrumIdProtocols = parser.GetSpectrumIdentificationProtocols().ToList();
				Assert.AreEqual(spectrumIdProtocols.Count, 2);
				Assert.AreEqual(spectrumIdProtocols[0].Name, "BioMarker");
				Assert.AreEqual(spectrumIdProtocols[0].SearchParams.UserParams.Count, 101);
			}
		}

		[Test]
		public void ParsedFragmentArraysTest()
		{
			using (var parser = new MzIdentMlParser(GoldenTDPortalPath))
			{
				List<MzIdentMlSpectrumIdentificationItem> spectrumIdItems = parser.GetSpectrumIdentificationItems().ToList();
				Assert.AreEqual(spectrumIdItems[0].IonTypes[0].MzArray.Values.Length, 11);
				Assert.AreEqual(spectrumIdItems[0].IonTypes[0].MzErrorArray.Values.Length, 11);
			}
		}

		[Test]
		public void ParseDatabaseSequenceParamsTest()
		{
			using (var parser = new MzIdentMlParser(GoldenTDPortalPath))
			{
				List<MzIdentMlDatabaseSequence> sequences = parser.GetDatabaseSequences().ToList();
				MzIdentMlDatabaseSequence sequence = sequences.First();
				Assert.AreEqual(sequence.ProteinDescription, "Transmembrane protein 14C");
				Assert.AreEqual(sequence.TaxonId, 9606);
			}
		}

		[Test]
		public void BasicTest()
		{
			using (var parser = new MzIdentMlParser(GetFilePath()))
			{
				var softwares = parser.GetAnalysisSoftware().ToList();
				Assert.IsNotNull(softwares);
				Assert.AreEqual(2, softwares.Count);
				Assert.AreEqual("AS_mascot_parser", softwares[1].Id);
				Assert.AreEqual("Mascot Parser", softwares[1].Name);
				Assert.AreEqual("http://www.matrixscience.com/msparser.html", softwares[1].Uri);
				Assert.AreEqual("2.3.0.0", softwares[1].Version);



				var databaseSequences = parser.GetDatabaseSequences().ToList();
				Assert.IsNotNull(databaseSequences);
				Assert.AreEqual(2, databaseSequences.Count);

				var dbSequence = databaseSequences[0];
				Assert.AreEqual("MYG_EQUBU", dbSequence.Accession);
				Assert.AreEqual(3, dbSequence.CvParams.Count);
				Assert.AreEqual("MS:1001088", dbSequence.CvParams[0].Accession);
				Assert.AreEqual("protein description", dbSequence.CvParams[0].Name);
				Assert.AreEqual("PSI-MS", dbSequence.CvParams[0].Reference);
				Assert.AreEqual("DBSeq_MYG_EQUBU", dbSequence.Id);
				Assert.AreEqual(154, dbSequence.Length);
				Assert.AreEqual("SDB_SwissProt", dbSequence.SearchDatabaseId);
				Assert.AreEqual("MGLSDGEWQQVLNVWGKVEADIAGHGQEVLIRLFTGHPETLEKFDKFKHLKTEAEMKASEDLKKHGTVVLTALGGILKKKGHHEAELKPLAQSHATKHKIPIKYLEFISDAIIHVLHSKHPGDFGADAQGAMTKALELFRNDIAAKYKELGFQG", dbSequence.Sequence);



				var peptides = parser.GetPeptides().ToList();
				Assert.IsNotNull(peptides);
				Assert.AreEqual(1, peptides.Count);

				var pep = peptides[0];
				Assert.AreEqual("DBSeq_MYG_EQUBU", pep.DatabaseSequences[0].Id);
				Assert.AreEqual("peptide_1", pep.Id);
				Assert.AreEqual(27, pep.Modifications[0].Location);
				Assert.AreEqual("UNIMOD:214", pep.Modifications[0].Accession);
				Assert.AreEqual(144.102063, pep.Modifications[0].MonoisotopicMassDelta);
				Assert.AreEqual("GLSDGEWQQVLNVWGKVEADIAGHGQEVLIRLFTGHPETLEKFDKFKHLKTEAEMKASEDLKKHGTVVLTALGGILKKKGHHEAELKPLAQSHATKHKIPIKYLEFISDAIIHVLHSKHPGDFGADAQGAMTKALELFRNDIAAKYKELGFQG", pep.Sequence);



				var evidences = parser.GetPeptideEvidences().ToList();
				Assert.IsNotNull(evidences);
				Assert.AreEqual(2, evidences.Count);

				var ev = evidences[0];
				Assert.AreEqual("DBSeq_MYG_EQUBU", ev.DatabaseSequence.Id);
				Assert.AreEqual(154, ev.End);
				Assert.AreEqual("PE_1_1_MYG_EQUBU_0", ev.Id);
				Assert.AreEqual(false, ev.IsDecoy);
				Assert.AreEqual("peptide_1", ev.Peptide.Id);
				Assert.AreEqual("-", ev.Post);
				Assert.AreEqual("M", ev.Pre);
				Assert.AreEqual(2, ev.Start);



				var inputs = parser.GetInputs();
				Assert.IsNotNull(inputs);
				Assert.AreEqual(1, inputs.SearchDatabases.Count);
				Assert.AreEqual(1, inputs.SourceFiles.Count);

				var database = inputs.SearchDatabases[0];
				Assert.AreEqual(1, database.CvParams.Count);
				Assert.AreEqual("MS:1001073", database.CvParams[0].Accession);
				Assert.IsNotNull(database.DatabaseName);
				Assert.IsNotNull(database.DatabaseName.UserParam);
				Assert.AreEqual("SwissProt_51.6.fasta", database.DatabaseName.UserParam.Name);
				Assert.AreEqual("file:///C:/inetpub/mascot/sequence/SwissProt/current/SwissProt_51.6.fasta", database.Location);
				Assert.AreEqual("SwissProt", database.Name);
				Assert.AreEqual(93947433, database.NumberOfResidues);
				Assert.AreEqual(257964, database.NumberOfSequences);
				Assert.AreEqual("SwissProt_51.6.fasta", database.Version);



				var measures = parser.GetFragmentationMeasures().ToList();
				Assert.IsNotNull(measures);
				Assert.AreEqual(3, measures.Count);

				var meas = measures[0];
				Assert.AreEqual("m_mz", meas.Id);
				Assert.AreEqual("MS:1001225", meas.Measure.Accession);
				Assert.AreEqual("product ion m/z", meas.Measure.Name);



				var proteinDetectionList = parser.GetProteinDetectionLists().Single();
				Assert.IsNotNull(proteinDetectionList);
				Assert.AreEqual("PDL_1", proteinDetectionList.Id);
				Assert.AreEqual(1, proteinDetectionList.ProteinAmbiguityGroups.Count);
				Assert.AreEqual(1, proteinDetectionList.ProteinAmbiguityGroups.Count);
				Assert.AreEqual("PAG_hit_1", proteinDetectionList.ProteinAmbiguityGroups[0].Id);
				Assert.AreEqual("PAG_hit_1", proteinDetectionList.ProteinAmbiguityGroups[0].Id);
				Assert.AreEqual(2, proteinDetectionList.ProteinAmbiguityGroups[0].ProteinDetectionHypotheses.Count);
				Assert.AreEqual(1, proteinDetectionList.ProteinAmbiguityGroups[0].ProteinDetectionHypotheses[0].PeptideHypotheses.Count);
				Assert.AreEqual(1, proteinDetectionList.ProteinAmbiguityGroups[0].ProteinDetectionHypotheses[0].PeptideHypotheses.Count);
				Assert.AreEqual("PE_1_1_MYG_EQUBU_0", proteinDetectionList.ProteinAmbiguityGroups[0].ProteinDetectionHypotheses[0].PeptideHypotheses[0].PeptideEvidenceId);
				Assert.AreEqual(3, proteinDetectionList.ProteinAmbiguityGroups[0].ProteinDetectionHypotheses[0].CvParams.Count);



				var proteinProtocols = parser.GetProteinDetectionProtocols().ToList();
				Assert.IsNotNull(proteinProtocols);
				Assert.AreEqual(1, proteinProtocols.Count);
				Assert.AreEqual(10, proteinProtocols[0].AnalysisParams.CvParams.Count);
				Assert.AreEqual("PDP_MascotParser_1", proteinProtocols[0].Id);
				Assert.AreEqual("AS_mascot_parser", proteinProtocols[0].SoftwareId);
				Assert.AreEqual(1, proteinProtocols[0].Thresholds.CvParams.Count);



				var spectrumIds = parser.GetSpectrumIdentificationItems().ToList();
				Assert.IsNotNull(spectrumIds);
				Assert.AreEqual(5, spectrumIds.Count);

				var id = spectrumIds[0];
				Assert.AreEqual(16941.97215, id.CalculatedMz);
				Assert.AreEqual(16947.854, id.ExperimentalMz);
				Assert.AreEqual(1, id.ChargeState);
				Assert.AreEqual(2, id.CvParams.Count);
				Assert.AreEqual("SII_1_1", id.Id);
				Assert.AreEqual(4, id.IonTypes.Count);
				Assert.AreEqual(1, id.IonTypes[0].Charge);
				Assert.AreEqual(1, id.IonTypes[0].CvParams.Count);
				Assert.AreEqual(3, id.IonTypes[0].FragmentArrays.Count);
				Assert.AreEqual(47, id.IonTypes[0].FragmentArrays[0].Values.Length);
				Assert.AreEqual(47, id.IonTypes[0].Indices.Length);
				Assert.AreEqual(true, id.PassesThreshold);
				Assert.AreEqual(2, id.PeptideEvidences.Count);
				Assert.AreEqual(2, id.Peptides.Count);
				Assert.AreEqual(1, id.Rank);

				var spectrumProtocols = parser.GetSpectrumIdentificationProtocols().ToList();
				Assert.IsNotNull(spectrumProtocols);
				Assert.AreEqual(1, spectrumProtocols.Count);

				var protocol = spectrumProtocols[0];
				Assert.AreEqual(1, protocol.DatabaseFilterParams.CvParams.Count);
				Assert.AreEqual(2, protocol.FragmentTolerances.CvParams.Count);
				Assert.AreEqual("SIP", protocol.Id);
				Assert.AreEqual(2, protocol.PrecursorTolerances.CvParams.Count);
				Assert.AreEqual(5, protocol.SearchParams.CvParams.Count);
				Assert.AreEqual(2, protocol.SearchParams.UserParams.Count);
				Assert.AreEqual("MS:1001083", protocol.SearchType.CvParam.Accession);
				Assert.AreEqual("AS_mascot_server", protocol.SoftwareId);
				Assert.AreEqual(1, protocol.Thresholds.CvParams.Count);
			}
		}
	}
}
