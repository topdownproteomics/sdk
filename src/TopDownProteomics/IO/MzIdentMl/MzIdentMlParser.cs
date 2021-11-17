using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Parser for MzIdentML files
	/// </summary>
	public class MzIdentMlParser : IDisposable
	{
		/// <summary>
		/// The protein description CV accession
		/// </summary>
		public static string ProteinDescriptionCvAccession = "MS:1001088";

		/// <summary>
		/// The taxonomy scientific name CV accession
		/// </summary>
		public static string TaxonomyScientificNameCvAccession = "MS:1001469";

		/// <summary>
		/// The taxonomy NCBI taxon ID CV accession
		/// </summary>
		public static string TaxonomyNcbiTaxonIdCvAccession = "MS:1001467";

		/// <summary>
		/// The product ion m/z CV accession
		/// </summary>
		public static string ProductIonMzCvAccession = "MS:1001225";

		/// <summary>
		/// The product ion m/z error CV accession
		/// </summary>
		public static string ProductIonMzErrorCvAccession = "MS:1001227";

		/// <summary>
		/// The product ion intensity CV accession
		/// </summary>
		public static string ProductIonIntensityCvAccession = "MS:1001226";

		/// <summary>
		/// Instantiates with a path
		/// </summary>
		/// <param name="path"></param>
		public MzIdentMlParser(string path)
		{
			this._path = path;
		}

		#region Methods
		#region Analysis Software

		/// <summary>
		/// Gets analysis software
		/// </summary>
		/// <returns></returns>
		public IEnumerable<MzIdentMlAnalysisSoftware> GetAnalysisSoftware()
		{
			using Stream fs = File.OpenRead(this._path);
			using XmlReader reader = XmlReader.Create(fs);
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "AnalysisSoftwareList")
					break;

				if (reader.NodeType == XmlNodeType.Element && reader.Name == "AnalysisSoftware")
				{
					yield return this.ParseAnalysisSoftware(reader);
				}
			}
		}

		private MzIdentMlAnalysisSoftware ParseAnalysisSoftware(XmlReader reader)
		{
			var id = reader.GetAttribute("id");
			if (string.IsNullOrEmpty(id))
				throw new Exception("AnalysisSoftware elements must contain an id");

			return new MzIdentMlAnalysisSoftware(id)
			{
				Name = reader.GetAttribute("name"),
				Version = reader.GetAttribute("version"),
				Uri = reader.GetAttribute("uri")
			};
		}
		#endregion
		#region Database Sequences

		/// <summary>
		/// Gets protein and/or isoform database sequences
		/// </summary>
		/// <returns></returns>
		public IEnumerable<MzIdentMlDatabaseSequence> GetDatabaseSequences()
		{
			using Stream fs = File.OpenRead(this._path);
			using XmlReader reader = XmlReader.Create(fs);
			foreach (var sequence in GetDatabaseSequencesAux(reader))
			{
				yield return sequence;
			}
		}

		private IEnumerable<MzIdentMlDatabaseSequence> GetDatabaseSequencesAux(XmlReader reader)
		{
			while (reader.Read())
			{
				if (reader.Name == "Peptide")
					break;

				if (reader.NodeType == XmlNodeType.Element && reader.Name == "DBSequence")
				{
					yield return this.ParseDatabaseSequence(reader);
				}
			}
		}

		private MzIdentMlDatabaseSequence ParseDatabaseSequence(XmlReader reader)
		{
			var id = reader.GetAttribute("id");
			var lengthAttribute = reader.GetAttribute("length");
			var databaseId = reader.GetAttribute("searchDatabase_ref");
			var accession = reader.GetAttribute("accession");

			string? sequence = null;
			var cvParams = new List<MzIdentMlCvParam>();
			var userParams = new List<MzIdentMlUserParam>();

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "DBSequence")
					break;

				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "Seq")
						sequence = reader.ReadInnerXml();

					else if (reader.Name == "cvParam")
						cvParams.Add(this.GetCvParam(reader));

					else if (reader.Name == "userParam")
						userParams.Add(this.GetUserParam(reader));
				}
			}

			if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(accession) || string.IsNullOrEmpty(databaseId))
				throw new Exception("DBSequence elements must contain an id, accession, and database ID");

			var databaseSequence = new MzIdentMlDatabaseSequence(id, accession, databaseId) { Sequence = sequence };

			if (int.TryParse(lengthAttribute, out int length))
				databaseSequence.Length = length;

			if (cvParams.Any())
			{
				databaseSequence.CvParams = cvParams;
				foreach (var cvParam in cvParams)
				{
					if (cvParam.Accession == ProteinDescriptionCvAccession)
					{
						databaseSequence.ProteinDescription = cvParam.Value;
					}
					else if (cvParam.Accession == TaxonomyNcbiTaxonIdCvAccession)
					{
						if (int.TryParse(cvParam.Value, out int taxonId))
						{
							databaseSequence.TaxonId = taxonId;
						}
						else
						{
							throw new Exception("Unable to parse taxon ID.");
						}
					}
					else if (cvParam.Accession == TaxonomyScientificNameCvAccession)
					{
						databaseSequence.TaxonomyScientificName = cvParam.Value;
					}
				}
			}
				

			if (userParams.Any())
				databaseSequence.UserParams = userParams;

			return databaseSequence;
		}

		#endregion
		#region Peptides
		/// <summary>
		/// Gets peptides, along with corresponding database sequences
		/// </summary>
		/// <returns></returns>
		public IEnumerable<MzIdentMlPeptide> GetPeptides()
		{
			return GetPeptideEvidences()
				.GroupBy(x => x.Peptide.Id)
				.Select(x => x.First().Peptide);
		}

		private IEnumerable<MzIdentMlPeptide> GetPeptidesAux(XmlReader reader)
		{
			do
			{
				if (reader.Name == "PeptideEvidence")
					break;

				if (reader.NodeType == XmlNodeType.Element && reader.Name == "Peptide")
				{
					yield return this.ParsePeptide(reader);
				}
			}
			while (reader.Read());
		}

		private MzIdentMlPeptide ParsePeptide(XmlReader reader)
		{
			var id = reader.GetAttribute("id");
			string? sequence = null;
			var modifications = new List<MzIdentMlModification>();
			var cvParams = new List<MzIdentMlCvParam>();
			var userParams = new List<MzIdentMlUserParam>();

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Peptide")
					break;
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "PeptideSequence")
						sequence = reader.ReadInnerXml();

					else if (reader.Name == "Modification")
					{
						modifications.Add(this.ParseModification(reader));
					}
					else if (reader.Name == "cvParam")
					{
						cvParams.Add(this.GetCvParam(reader));
					}
					else if (reader.Name == "cvParam")
					{
						userParams.Add(this.GetUserParam(reader));
					}
				}
			}

			if (string.IsNullOrEmpty(sequence) || string.IsNullOrEmpty(id))
				throw new Exception("Error parsing peptide");

			var peptide = new MzIdentMlPeptide(id, sequence);

			if (modifications.Any())
				peptide.Modifications = modifications;
			if (cvParams.Any())
				peptide.CvParams = cvParams;
			if (userParams.Any())
				peptide.UserParams = userParams;

			return peptide;
		}

		private MzIdentMlModification ParseModification(XmlReader reader)
		{
			string monoisotopicMassAttribute = reader.GetAttribute("monoisotopicMassDelta");
			string locationAttribute = reader.GetAttribute("location");

			string? accession = null;
			string? name = null;
			string? cvRef = null;

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Modification")
					break;

				else if (reader.NodeType == XmlNodeType.Element && reader.Name == "cvParam")
				{
					accession = reader.GetAttribute("accession");
					name = reader.GetAttribute("name");
					cvRef = reader.GetAttribute("cvRef");
				}
			}

			if (string.IsNullOrEmpty(accession) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(cvRef))
				throw new Exception("Modification elements require an accession, name, and cvRef");

			var mod = new MzIdentMlModification(accession, name, cvRef);

			if (double.TryParse(monoisotopicMassAttribute, out double mass))
				mod.MonoisotopicMassDelta = mass;

			if (int.TryParse(locationAttribute, out int location))
				mod.Location = location;

			return mod;
		}
		#endregion
		#region Peptide Evidences

		/// <summary>
		/// Gets evidence linking peptides to specific database sequences
		/// </summary>
		/// <returns></returns>
		public IEnumerable<MzIdentMlPeptideEvidence> GetPeptideEvidences()
		{
			using Stream fs = File.OpenRead(this._path);
			using XmlReader reader = XmlReader.Create(fs);
			foreach (MzIdentMlPeptideEvidence evidence in GetPeptideEvidencesAux(reader))
			{
				yield return evidence;
			}
		}

		private IEnumerable<MzIdentMlPeptideEvidence> GetPeptideEvidencesAux(XmlReader reader)
		{
			Dictionary<string, MzIdentMlDatabaseSequence> databaseSequences = GetDatabaseSequencesAux(reader)
				.ToDictionary(x => x.Id, x => x);
			Dictionary<string, MzIdentMlPeptide> peptides = GetPeptidesAux(reader)
				.ToDictionary(x => x.Id, x => x);

			do
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "SequenceCollection")
					break;

				if (reader.NodeType == XmlNodeType.Element && reader.Name == "PeptideEvidence")
				{
					yield return this.ParsePeptideEvidences(reader, peptides, databaseSequences);
				}
			}
			while (reader.Read());
		}

		private MzIdentMlPeptideEvidence ParsePeptideEvidences(XmlReader reader, Dictionary<string, MzIdentMlPeptide> peptides, Dictionary<string, MzIdentMlDatabaseSequence> databaseSequences)
		{
			string id = reader.GetAttribute("id");
			if (string.IsNullOrEmpty(id))
				throw new Exception("PeptideEvidence elements must have an id");

			string peptideId = reader.GetAttribute("peptide_ref");
			if (!peptides.TryGetValue(peptideId, out MzIdentMlPeptide peptide))
			{
				throw new Exception($"No peptide found for PeptideEvidence peptide_ref {peptideId}");
			}

			string databaseSequenceId = reader.GetAttribute("dBSequence_ref");
			if (!databaseSequences.TryGetValue(databaseSequenceId, out MzIdentMlDatabaseSequence dbSequence))
			{
				throw new Exception($"No DBSequence found for PeptideEvidence dBSequence_ref {databaseSequenceId}");
			}			

			var peptideEvidence = new MzIdentMlPeptideEvidence(dbSequence, id, peptide)
			{
				Pre = reader.GetAttribute("pre"),
				Post = reader.GetAttribute("post")
			};

			if (int.TryParse(reader.GetAttribute("start"), out int start))
				peptideEvidence.Start = start;

			if (int.TryParse(reader.GetAttribute("end"), out int end))
				peptideEvidence.End = end;

			if (bool.TryParse(reader.GetAttribute("isDecoy"), out bool isDecoy))
				peptideEvidence.IsDecoy = isDecoy;


			peptide.DatabaseSequences ??= new List<MzIdentMlDatabaseSequence>();
			peptide.DatabaseSequences.Add(dbSequence);

			peptide.PeptideEvidences ??= new List<MzIdentMlPeptideEvidence>();
			peptide.PeptideEvidences.Add(peptideEvidence);

			return peptideEvidence;

			
		}
		#endregion
		#region Spectrum Identification Protocol

		/// <summary>
		/// Gets search engine protocols
		/// </summary>
		/// <returns></returns>
		public IEnumerable<MzIdentMlSpectrumIdentificationProtocol> GetSpectrumIdentificationProtocols()
		{
			using Stream fs = File.OpenRead(this._path);
			using XmlReader reader = XmlReader.Create(fs);
			foreach (var protocol in GetSpectrumIdentificationProtocolsAux(reader))
			{
				yield return protocol;
			}
		}

		private IEnumerable<MzIdentMlSpectrumIdentificationProtocol> GetSpectrumIdentificationProtocolsAux(XmlReader reader)
		{
			do
			{
				if (reader.NodeType == XmlNodeType.Element && reader.Name == "ProteinDetectionProtocol")
					break;

				if (reader.NodeType == XmlNodeType.Element && reader.Name == "SpectrumIdentificationProtocol")
				{
					yield return this.ParseSpectrumIdentificationProtocol(reader);
				}
			}
			while (reader.Read());
		}

		private MzIdentMlSpectrumIdentificationProtocol ParseSpectrumIdentificationProtocol(XmlReader reader)
		{
			var id = reader.GetAttribute("id");
			var softwareId = reader.GetAttribute("analysisSoftware_ref");
			string name = reader.GetAttribute("name");

			MzIdentMlCvOrUserParam? searchType = null;

			MzIdentMlParamCollection? thresholds = null;
			MzIdentMlParamCollection? searchParams = null;
			MzIdentMlParamCollection? databaseFilterParams = null;
			MzIdentMlParamCollection? fragmentTolerances = null;
			MzIdentMlParamCollection? precursorTolerances = null;

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "SpectrumIdentificationProtocol")
					break;

				if (reader.NodeType == XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case ("SearchType"):
							searchType = this.GetCvOrUserParam(reader, reader.Name);
							break;
						case ("AdditionalSearchParams"):
							searchParams = this.GetParamCollection(reader, reader.Name);
							break;
						case ("FragmentTolerance"):
							fragmentTolerances = this.GetParamCollection(reader, reader.Name);
							break;
						case ("ParentTolerance"):
							precursorTolerances = this.GetParamCollection(reader, reader.Name);
							break;
						case ("Threshold"):
							thresholds = this.GetParamCollection(reader, reader.Name);
							break;
						case ("DatabaseFilters"):
							databaseFilterParams = this.GetParamCollection(reader, reader.Name);
							break;
						default:
							break;
					}
				}
			}

			if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(softwareId) || thresholds is null || searchType is null)
				throw new Exception("SpectrumIdentificationProtocol elements must contain an id, software reference, thresholds, and search type");

			var protocol = new MzIdentMlSpectrumIdentificationProtocol(id, softwareId, searchType, thresholds, name)
			{
				SearchParams = searchParams,
				FragmentTolerances = fragmentTolerances,
				PrecursorTolerances = precursorTolerances,
				DatabaseFilterParams = databaseFilterParams
			};

			return protocol;
		}
		#endregion
		#region Protein Detection Protocol

		/// <summary>
		/// Gets search engine protocols used for linking peptides to database sequences
		/// </summary>
		/// <returns></returns>
		public IEnumerable<MzIdentMlProteinDetectionProtocol> GetProteinDetectionProtocols()
		{
			using Stream fs = File.OpenRead(this._path);
			using XmlReader reader = XmlReader.Create(fs);
			foreach (var protocol in GetProteinDetectionProtocolsAux(reader))
			{
				yield return protocol;
			}
		}

		private IEnumerable<MzIdentMlProteinDetectionProtocol> GetProteinDetectionProtocolsAux(XmlReader reader)
		{
			do
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "AnalysisProtocolCollection")
					break;

				if (reader.NodeType == XmlNodeType.Element && reader.Name == "ProteinDetectionProtocol")
				{
					yield return this.ParseProteinDetectionProtocol(reader);
				}
			}
			while (reader.Read());
		}

		private MzIdentMlProteinDetectionProtocol ParseProteinDetectionProtocol(XmlReader reader)
		{
			var id = reader.GetAttribute("id");
			var softwareId = reader.GetAttribute("analysisSoftware_ref");
			var name = reader.GetAttribute("name");

			MzIdentMlParamCollection? thresholds = null;
			MzIdentMlParamCollection? analysisParams = null;

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "ProteinDetectionProtocol")
					break;

				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "AnalysisParams")
						analysisParams = this.GetParamCollection(reader, reader.Name);

					else if (reader.Name == "Threshold")
						thresholds = this.GetParamCollection(reader, reader.Name);
				}
			}

			if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(softwareId) || thresholds is null)
				throw new Exception("ProteinDetectionProtocol element must have an id, software reference, and thresholds");

			var protocol = new MzIdentMlProteinDetectionProtocol(id, softwareId, thresholds)
			{
				Name = name,
				AnalysisParams = analysisParams
			};

			return protocol;
		}
		#endregion
		#region Inputs

		/// <summary>
		/// Gets the spectra and database inputs
		/// </summary>
		/// <returns></returns>
		public MzIdentMlInputs GetInputs()
		{
			using Stream fs = File.OpenRead(this._path);
			using XmlReader reader = XmlReader.Create(fs);
			return GetInputsAux(reader);
		}

		private MzIdentMlInputs GetInputsAux(XmlReader reader)
		{
			var sourceFiles = new List<MzIdentMlSourceFile>();
			var searchDatabases = new List<MzIdentMlSearchDatabase>();
			MzIdentMlSpectraData? spectraData = null;

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Inputs")
					break;

				if (reader.NodeType == XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case "SourceFile":
							sourceFiles.Add(this.ParseSourceFile(reader));
							break;
						case "SearchDatabase":
							searchDatabases.Add(this.ParseSearchDatabase(reader));
							break;
						case "SpectraData":
							spectraData = this.ParseInputSpectraData(reader);
							break;
						default:
							break;
					}
				}
			}

			if (spectraData == null)
				throw new Exception("SpectraData element not found.");

			var input = new MzIdentMlInputs(spectraData);

			if (sourceFiles.Any())
				input.SourceFiles = sourceFiles;

			if (searchDatabases.Any())
				input.SearchDatabases = searchDatabases;

			return input;
		}

		private MzIdentMlSourceFile ParseSourceFile(XmlReader reader)
		{
			var id = reader.GetAttribute("id");
			var location = reader.GetAttribute("location");

			MzIdentMlCvParam? fileFormat = null;
			var userParams = new List<MzIdentMlUserParam>();
			var cvParams = new List<MzIdentMlCvParam>();

			if (!reader.IsEmptyElement)
			{
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "SourceFile")
						break;

					if (reader.NodeType == XmlNodeType.Element)
					{
						if (reader.Name == "FileFormat")
						{
							// only one cvParam allowed per FileFormat
							fileFormat = this.GetNestedCvParams(reader, "FileFormat").First();
						}

						else if (reader.Name == "userParam")
							userParams.Add(this.GetUserParam(reader));

						else if (reader.Name == "cvParam")
							cvParams.Add(this.GetCvParam(reader));
					}
				}
			}

			if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(location))
				throw new Exception("SourceFile must have an id and location");

			var sourceFile = new MzIdentMlSourceFile(id, location) { FileFormat = fileFormat };

			if (userParams.Any())
				sourceFile.UserParams = userParams;

			if (cvParams.Any())
				sourceFile.CvParams = cvParams;

			return sourceFile;
		}

		private MzIdentMlSearchDatabase ParseSearchDatabase(XmlReader reader)
		{
			var id = reader.GetAttribute("id");
			var location = reader.GetAttribute("location");
			var name = reader.GetAttribute("name");
			var sequenceCountAttribute = reader.GetAttribute("numDatabaseSequences");
			var residueCountAttribute = reader.GetAttribute("numResidues");
			var version = reader.GetAttribute("version");
			var releaseDate = reader.GetAttribute("releaseDate");

			MzIdentMlCvParam ? fileFormat = null;
			MzIdentMlCvOrUserParam? databaseName = null;
			var cvParams = new List<MzIdentMlCvParam>();

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "SearchDatabase")
					break;

				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "FileFormat")
					{
						// only one cvParam allowed per FileFormat
						fileFormat = this.GetNestedCvParams(reader, "FileFormat").First();
					}

					else if (reader.Name == "DatabaseName")
					{
						databaseName = this.GetCvOrUserParam(reader, reader.Name);
					}

					else if (reader.Name == "cvParam")
					{
						cvParams.Add(this.GetCvParam(reader));
					}
				}
			}

			if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(location))
				throw new Exception("SearchDatabase elements must contain an id and location");

			var database = new MzIdentMlSearchDatabase(id, location)
			{
				DatabaseName = databaseName,
				FileFormat = fileFormat,
				Name = name,
				Version = version,
				ReleaseDate = releaseDate
			};

			if (int.TryParse(sequenceCountAttribute, out int sequenceCount))
				database.NumberOfSequences = sequenceCount;

			if (int.TryParse(residueCountAttribute, out int residueCount))
				database.NumberOfResidues = residueCount;

			if (cvParams.Any())
				database.CvParams = cvParams;

			return database;
		}

		private MzIdentMlSpectraData ParseInputSpectraData(XmlReader reader)
		{
			var id = reader.GetAttribute("id");
			var location = reader.GetAttribute("location");

			MzIdentMlCvParam? fileFormat = null;
			MzIdentMlCvParam? spectrumIdFormat = null;

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "SpectraData")
					break;

				if (reader.NodeType == XmlNodeType.Element && reader.Name == "FileFormat")
				{
					// only one cvParam allowed per FileFormat
					fileFormat = this.GetNestedCvParams(reader, "FileFormat").First();
				}
				else if (reader.NodeType == XmlNodeType.Element && reader.Name == "SpectrumIDFormat")
				{
					// only one cvParam allowed per SpectrumIDFormat
					spectrumIdFormat = this.GetNestedCvParams(reader, "SpectrumIDFormat").First();
				}
			}

			if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(location) || spectrumIdFormat is null)
				throw new Exception("SpectraData elements must have an id, location, and spectrumIdFormat");

			var spectraData = new MzIdentMlSpectraData(id, location, spectrumIdFormat);

			if (fileFormat != null)
				spectraData.FileFormat = fileFormat;

			return spectraData;
		}
		#endregion
		#region Analysis Data

		/// <summary>
		/// Gets the fragmentation measures used in the spectrum identification ion arrays
		/// </summary>
		/// <returns></returns>
		public IEnumerable<MzIdentMlFragmentationMeasure> GetFragmentationMeasures()
		{
			using Stream fs = File.OpenRead(this._path);
			using XmlReader reader = XmlReader.Create(fs);
			foreach (var measure in GetFragmentationMeasuresAux(reader))
			{
				yield return measure;
			}
		}

		private IEnumerable<MzIdentMlFragmentationMeasure> GetFragmentationMeasuresAux(XmlReader reader)
		{
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "FragmentationTable")
					break;

				if (reader.NodeType == XmlNodeType.Element && reader.Name == "Measure")
				{
					yield return ParseMeasure(reader);
				}
			}
		}

		private MzIdentMlFragmentationMeasure ParseMeasure(XmlReader reader)
		{
			var id = reader.GetAttribute("id");

			// only one cvParam per spec doc
			var cvParam = this.GetNestedCvParams(reader, "Measure").Single();
			return new MzIdentMlFragmentationMeasure(id, cvParam);
		}

		/// <summary>
		/// Gets the spectrum identification items
		/// </summary>
		/// <returns></returns>
		public IEnumerable<MzIdentMlSpectrumIdentificationItem> GetSpectrumIdentificationItems()
		{
			foreach (MzIdentMlSpectrumIdentificationList list in GetSpectrumIdentificationLists())
			{
				foreach (MzIdentMlSpectrumIdentificationResult result in list.SpectrumIdentificationResults)
				{
					foreach (MzIdentMlSpectrumIdentificationItem spectrumIdItem in result.SpectrumIdentificationItems)
					{
						yield return spectrumIdItem;
					}
				}
			}
		}

		/// <summary>
		/// Gets the spectrum identification lists
		/// </summary>
		/// <returns></returns>
		public IEnumerable<MzIdentMlSpectrumIdentificationList> GetSpectrumIdentificationLists()
		{
			using Stream fs = File.OpenRead(this._path);
			using XmlReader reader = XmlReader.Create(fs);
			foreach (MzIdentMlSpectrumIdentificationList list in GetSpectrumIdentificationListsAux(reader))
			{
				yield return list;
			}
		}

		private IEnumerable<MzIdentMlSpectrumIdentificationList> GetSpectrumIdentificationListsAux(XmlReader reader)
		{
			Dictionary<string, MzIdentMlPeptideEvidence> peptideEvidences = GetPeptideEvidencesAux(reader)
				.ToDictionary(x => x.Id, x => x);

			MzIdentMlAnalysisCollection analysisCollection = GetAnalysisCollection(reader);

			List<MzIdentMlSpectrumIdentificationProtocol> spectrumProtocols = GetSpectrumIdentificationProtocolsAux(reader).ToList();
			List<MzIdentMlProteinDetectionProtocol> proteinProtocols = GetProteinDetectionProtocolsAux(reader).ToList();

			MzIdentMlSpectraData spectraData = GetInputsAux(reader).SpectraData;

			List<MzIdentMlSpectrumIdentificationList> spectrumIdLists = new List<MzIdentMlSpectrumIdentificationList>();

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "ProteinDetectionList")
					{
						break;
					}

					if (reader.Name == "SpectrumIdentificationList")
					{
						MzIdentMlSpectrumIdentificationList spectrumIdList = ParseSpectrumIdentificationList(reader, peptideEvidences, analysisCollection, spectrumProtocols, proteinProtocols, spectraData);
						yield return spectrumIdList;
					}
				}
				else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "SpectrumIdentificationList")
					break;

			}
		}

		private MzIdentMlAnalysisCollection GetAnalysisCollection(XmlReader reader)
		{
			List<MzIdentMlSpectrumIdentification> spectrumIds = new List<MzIdentMlSpectrumIdentification>();
			List<MzIdentMlProteinDetection> proteinDetections = new List<MzIdentMlProteinDetection>();
			do
			{
				if (reader.Name == "AnalysisCollection" && reader.NodeType == XmlNodeType.EndElement)
				{
					break;
				}

				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "SpectrumIdentification")
					{
						spectrumIds.Add(ParseSpectrumIdentification(reader));
					}
					else if (reader.Name == "ProteinDetection")
					{
						proteinDetections.Add(ParseProteinDetection(reader));
					}
				}
			} while (reader.Read());

			if (!spectrumIds.Any())
				throw new Exception("Spectrum identifications not found.");

			return new MzIdentMlAnalysisCollection(spectrumIds, proteinDetections);
		}

		private MzIdentMlSpectrumIdentification ParseSpectrumIdentification(XmlReader reader)
		{
			string id = reader.GetAttribute("id");
			string protocolId = reader.GetAttribute("spectrumIdentificationProtocol_ref");
			string idListId = reader.GetAttribute("spectrumIdentificationList_ref");

			string? spectraId = null;
			string? databaseId = null;
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "SpectrumIdentification")
					break;

				if (reader.Name == "InputSpectra" && reader.NodeType == XmlNodeType.Element)
				{
					spectraId = reader.GetAttribute("spectraData_ref");
				}

				else if (reader.Name == "SearchDatabaseRef")
				{
					databaseId = reader.GetAttribute("searchDatabase_ref");
				}
			}

			if (spectraId is null || databaseId is null)
				throw new Exception("Spectrum identification spectra ID and database ID not found.");

			return new MzIdentMlSpectrumIdentification(id, protocolId, idListId, spectraId, databaseId);
		}

		private MzIdentMlProteinDetection ParseProteinDetection(XmlReader reader)
		{
			string id = reader.GetAttribute("id");
			string protocolId = reader.GetAttribute("proteinDetectionProtocol_ref");
			string detectionListId = reader.GetAttribute("proteinDetectionList_ref");
			string? spectrumIdListId = null;

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "ProteinDetection")
					break;

				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "InputSpectrumIdentifications")
					{
						spectrumIdListId = reader.GetAttribute("spectrumIdentificationList_ref");
					}
				}
			}

			if (spectrumIdListId is null)
				throw new Exception("Protein detection spectrum identification list ID not found.");

			return new MzIdentMlProteinDetection(id, protocolId, detectionListId, spectrumIdListId);
		}

		private MzIdentMlSpectrumIdentificationList ParseSpectrumIdentificationList(XmlReader reader, Dictionary<string, MzIdentMlPeptideEvidence> peptideEvidences, MzIdentMlAnalysisCollection analysisCollection, IEnumerable<MzIdentMlSpectrumIdentificationProtocol> spectrumProtocols, IEnumerable<MzIdentMlProteinDetectionProtocol> proteinProtocols, MzIdentMlSpectraData spectraData)
		{
			string id = reader.GetAttribute("id");

			MzIdentMlProteinDetectionProtocol proteinProtocol = GetProteinDetectionProtocol(analysisCollection, proteinProtocols, id);
			MzIdentMlSpectrumIdentificationProtocol spectrumProtocol = GetSpectrumIdentificationProtocol(analysisCollection, spectrumProtocols, id);

			string? name = reader.GetAttribute("name");
			if (string.IsNullOrEmpty(name))
				name = null;


			int? numSequencesSearched = null;
			if (int.TryParse(reader.GetAttribute("numSequencesSearched"), out int sequencesSearched))
			{
				numSequencesSearched = sequencesSearched;
			}

			Dictionary<string, MzIdentMlFragmentationMeasure> measures = new Dictionary<string, MzIdentMlFragmentationMeasure>();
			List<MzIdentMlSpectrumIdentificationResult> results = new List<MzIdentMlSpectrumIdentificationResult>();

			while (reader.Read())
			{
				if (reader.Name == "SpectrumIdentificationList" && reader.NodeType == XmlNodeType.EndElement)
					break;

				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "Measure")
					{
						MzIdentMlFragmentationMeasure measure = ParseMeasure(reader);
						measures[measure.Id] = measure;
					}

					else if (reader.Name == "SpectrumIdentificationResult")
					{
						MzIdentMlSpectrumIdentificationResult result = ParseSpectrumIdentificationResult(reader, peptideEvidences, measures, spectraData);
						results.Add(result);
					}
				}
			}

			return new MzIdentMlSpectrumIdentificationList(id, measures.Values.ToList(), results, proteinProtocol, spectrumProtocol, name, numSequencesSearched);
		}

		private MzIdentMlProteinDetectionProtocol GetProteinDetectionProtocol(MzIdentMlAnalysisCollection analysisCollection, IEnumerable<MzIdentMlProteinDetectionProtocol> proteinProtocols, string spectrumListId)
		{
			string? proteinProtocolId = analysisCollection.ProteinDetections.FirstOrDefault(x => x.SpectrumIdentificationListId == spectrumListId).ProteinDetectionProtocolId;
			if (proteinProtocolId != null)
			{
				return proteinProtocols.FirstOrDefault(x => x.Id == proteinProtocolId);
			}

			throw new Exception("Unable to find protein detection protocol for spectrum identification lists.");
		}

		private MzIdentMlSpectrumIdentificationProtocol GetSpectrumIdentificationProtocol(MzIdentMlAnalysisCollection analysisCollection, IEnumerable<MzIdentMlSpectrumIdentificationProtocol> spectrumProtocols, string spectrumListId)
		{

			string? spectrumProtocolId = analysisCollection.SpectrumIdentifications.FirstOrDefault(x => x.SpectrumIdentificationListId == spectrumListId).SpectrumIdentificationProtocolId;
			if (spectrumProtocolId != null)
			{
				return spectrumProtocols.FirstOrDefault(x => x.Id == spectrumProtocolId);
			}

			throw new Exception("Unable to find spectrum identification protocol for spectrum identification lists.");
		}

		private MzIdentMlSpectrumIdentificationResult ParseSpectrumIdentificationResult(XmlReader reader, Dictionary<string, MzIdentMlPeptideEvidence> peptideEvidences, Dictionary<string, MzIdentMlFragmentationMeasure> measures, MzIdentMlSpectraData spectraData)
		{
			string id = reader.GetAttribute("id");
			string spectrumId = reader.GetAttribute("spectrumID");
			string inputSpectraDataId = reader.GetAttribute("spectraData_ref");

			var spectrumIdentificationItems = new List<MzIdentMlSpectrumIdentificationItem>();
			var cvParams = new List<MzIdentMlCvParam>();
			var userParams = new List<MzIdentMlUserParam>();

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "SpectrumIdentificationResult")
					break;

				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "SpectrumIdentificationItem")
						spectrumIdentificationItems.Add(this.ParseSpectrumIdentificationItem(reader, peptideEvidences, measures));

					else if (reader.Name == "cvParam")
						cvParams.Add(this.GetCvParam(reader));

					else if (reader.Name == "userParam")
						userParams.Add(this.GetUserParam(reader));
				}
			}

			if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(spectrumId) || string.IsNullOrEmpty(inputSpectraDataId) || spectrumIdentificationItems.Count == 0)
				throw new Exception("SpectrumIdentificationResult elements require SpectrumIdentificationItems and an id, spectrumID, and spectraData_ref.");

			if (inputSpectraDataId != spectraData.Id)
				throw new Exception("SpectrumIdentificationResult spectra data ID does not match spectral data provided in Inputs.");

			int? scanNumber = null;
			if (int.TryParse(spectrumId.Split("scan = ").Last(), out int scan))
			{
				scanNumber = scan;
			}

			var spectrumIdentificationResult = new MzIdentMlSpectrumIdentificationResult(id, spectrumId, scanNumber, spectraData, inputSpectraDataId, spectrumIdentificationItems);

			if (cvParams.Any())
				spectrumIdentificationResult.CvParams = cvParams;

			if (userParams.Any())
				spectrumIdentificationResult.UserParams = userParams;

			return spectrumIdentificationResult;
		}

		private MzIdentMlSpectrumIdentificationItem ParseSpectrumIdentificationItem(XmlReader reader, Dictionary<string, MzIdentMlPeptideEvidence> peptideEvidences, Dictionary<string, MzIdentMlFragmentationMeasure> measures)
		{
			var id = reader.GetAttribute("id");
			var calculatedMzAttribute = reader.GetAttribute("calculatedMassToCharge");
			var chargeStateAttribute = reader.GetAttribute("chargeState");
			var experimentalMzAttribute = reader.GetAttribute("experimentalMassToCharge");
			var rankAttribute = reader.GetAttribute("rank");
			var passesThresholdAttribute = reader.GetAttribute("passThreshold");

			var evidences = new List<MzIdentMlPeptideEvidence>();
			var ionTypes = new List<MzIdentMlIonType>();
			var cvParams = new List<MzIdentMlCvParam>();
			var userParams = new List<MzIdentMlUserParam>();

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "SpectrumIdentificationItem")
					break;

				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "PeptideEvidenceRef")
					{
						string peptideEvidenceId = reader.GetAttribute("peptideEvidence_ref");
						if (!peptideEvidences.TryGetValue(peptideEvidenceId, out var peptideEvidence))
						{
							throw new Exception($"Unable to find peptide evidence for SpectrumIdentificationItem {id}");
						}
						evidences.Add(peptideEvidence);
					}
						
					else if (reader.Name == "IonType")
						ionTypes.Add(this.ParseIonType(reader, measures));

					else if (reader.Name == "cvParam")
						cvParams.Add(this.GetCvParam(reader));

					else if (reader.Name == "userParam")
						userParams.Add(this.GetUserParam(reader));
				}
			}

			if (string.IsNullOrEmpty(id) ||
				double.TryParse(calculatedMzAttribute, out double calculatedMz) == false ||
				int.TryParse(chargeStateAttribute, out int chargeState) == false ||
				double.TryParse(experimentalMzAttribute, out double experimentalMz) == false ||
				int.TryParse(rankAttribute, out int rank) == false ||
				bool.TryParse(passesThresholdAttribute, out bool passesThreshold) == false ||
				evidences.Count() == 0)

				throw new Exception("SpectrumIdentificationItem elements must contain PeptideEvidenceRefs and an id, calculatedMz, chargeState, experimentalMz, rank, and passesThreshold");

			var spectrumId = new MzIdentMlSpectrumIdentificationItem(id, chargeState, calculatedMz, experimentalMz, passesThreshold, rank, evidences);

			if (ionTypes.Any())
				spectrumId.IonTypes = ionTypes;

			if (cvParams.Any())
				spectrumId.CvParams = cvParams;

			if (userParams.Any())
				spectrumId.UserParams = userParams;

			return spectrumId;
		}

		private MzIdentMlIonType ParseIonType(XmlReader reader, Dictionary<string, MzIdentMlFragmentationMeasure> measures)
		{
			var indices = this.ParseIonTypeIndices(reader.GetAttribute("index"));
			var chargeAttribute = reader.GetAttribute("charge");

			var fragmentArrays = new List<MzIdentMlFragmentArray>();
			var cvParams = new List<MzIdentMlCvParam>();

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "IonType")
					break;

				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "FragmentArray")
					{
						fragmentArrays.Add(this.ParseFragmentArray(reader, measures));
					}

					else if (reader.Name == "cvParam")
					{
						cvParams.Add(this.GetCvParam(reader));
					}
				}
			}

			if (cvParams.Count == 0 || int.TryParse(chargeAttribute, out int charge) == false)
				throw new Exception("IonType elements must contain cvParams and charge");

			var ionType = new MzIdentMlIonType(cvParams, charge);

			if (indices.Length > 0)
				ionType.Indices = indices;

			if (fragmentArrays.Any())
			{
				ionType.FragmentArrays = fragmentArrays;
				foreach (MzIdentMlFragmentArray array in ionType.FragmentArrays)
				{
					if (array.Measure.Measure.Accession == ProductIonMzCvAccession)
					{
						ionType.MzArray = array;
					}
					else if (array.Measure.Measure.Accession == ProductIonIntensityCvAccession)
					{
						ionType.IntensityArray = array;
					}
					else if (array.Measure.Measure.Accession == ProductIonMzErrorCvAccession)
					{
						ionType.MzErrorArray = array;
					}
				}
			}
				
			return ionType;
		}

		private int[] ParseIonTypeIndices(string indexString)
		{
			try
			{
				return indexString.Split(' ', StringSplitOptions.RemoveEmptyEntries)
				.Select(x => Convert.ToInt32(x))
				.ToArray();
			}
			catch (Exception e)
			{
				throw new Exception("Spectra indices must be numeric", e);
			}
		}

		private MzIdentMlFragmentArray ParseFragmentArray(XmlReader reader, Dictionary<string, MzIdentMlFragmentationMeasure> measures)
		{
			double[] values = this.ParseFragmentValues(reader.GetAttribute("values"));
			var measureId = reader.GetAttribute("measure_ref");

			if (values.Length == 0 || string.IsNullOrEmpty(measureId))
				throw new Exception("FragmentArray elements must contain values and a measure_ref");

			if (measures.ContainsKey(measureId))
			{
				return new MzIdentMlFragmentArray(measures[measureId], values);
			}

			throw new Exception($"Unable to find measure for {measureId}");
		}

		private double[] ParseFragmentValues(string spectraString)
		{
			try
			{
				return spectraString.Split(' ', StringSplitOptions.RemoveEmptyEntries)
				.Select(x => Convert.ToDouble(x))
				.ToArray();
			}
			catch (Exception e)
			{
				throw new Exception("Fragment values must be numeric", e);
			}
		}
		#endregion
		#region Protein Detection List

		/// <summary>
		/// Gets results linking peptides to proteins and/or protein groups
		/// </summary>
		/// <returns></returns>
		public IEnumerable<MzIdentMlProteinDetectionList> GetProteinDetectionLists()
		{
			MzIdentMlProteinDetectionList? proteinDetectionList = null;

			using Stream fs = File.OpenRead(this._path);
			using XmlReader reader = XmlReader.Create(fs);
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "AnalysisData")
					break;

				if (reader.NodeType == XmlNodeType.Element && reader.Name == "ProteinDetectionList")
				{
					var id = reader.GetAttribute("id");
					if (string.IsNullOrEmpty(id))
						throw new Exception("ProteinDetectionList must contain an ID");

					proteinDetectionList = new MzIdentMlProteinDetectionList(id)
					{
						Name = reader.GetAttribute("name"),
						ProteinAmbiguityGroups = this.ParseProteinDetectionList(reader).ToList()
					};
					yield return proteinDetectionList;
				}
			}
		}

		private IEnumerable<MzIdentMlProteinAmbiguityGroup> ParseProteinDetectionList(XmlReader reader)
		{
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "ProteinDetectionList")
					break;

				if (reader.NodeType == XmlNodeType.Element && reader.Name == "ProteinAmbiguityGroup")
				{
					var id = reader.GetAttribute("id");
					var hypotheses = this.ParseProteinDetectionHypotheses(reader).ToList();

					if (id is null || hypotheses is null)
						throw new Exception("ProteinAmbiguityGroup elements require an id and ProteinDetectionHypothesis");

					yield return new MzIdentMlProteinAmbiguityGroup(id, hypotheses);
				}
			}
		}

		private IEnumerable<MzIdentMlProteinDetectionHypothesis> ParseProteinDetectionHypotheses(XmlReader reader)
		{
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "ProteinAmbiguityGroup")
					break;

				if (reader.NodeType == XmlNodeType.Element && reader.Name == "ProteinDetectionHypothesis")
				{
					yield return this.ParseProteinHypothesis(reader);
				}
			}
		}

		private MzIdentMlProteinDetectionHypothesis ParseProteinHypothesis(XmlReader reader)
		{
			var id = reader.GetAttribute("id");
			var databaseSequenceId = reader.GetAttribute("dBSequence_ref");
			var passesThresholdAttribute = reader.GetAttribute("passThreshold");

			var peptideHypotheses = new List<MzIdentMlPeptideHypothesis>();
			var userParams = new List<MzIdentMlUserParam>();
			var cvParams = new List<MzIdentMlCvParam>();

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "ProteinDetectionHypothesis")
					break;

				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "PeptideHypothesis")
						peptideHypotheses.Add(this.ParsePeptideHypothesis(reader));

					else if (reader.Name == "userParam")
						userParams.Add(this.GetUserParam(reader));

					else if (reader.Name == "cvParam")
						cvParams.Add(this.GetCvParam(reader));
				}
			}

			if (string.IsNullOrEmpty(id) ||
				string.IsNullOrEmpty(databaseSequenceId) ||
				bool.TryParse(passesThresholdAttribute, out bool passesThreshold) == false ||
				peptideHypotheses.Count() == 0)

				throw new Exception("ProteinDetectionHypothesis elements require an id, databaseSequenceRef, passesThreshold, and PeptideHypothesis");


			return new MzIdentMlProteinDetectionHypothesis(id, databaseSequenceId, passesThreshold, peptideHypotheses)
			{
				CvParams = cvParams,
				UserParams = userParams
			};
		}

		private MzIdentMlPeptideHypothesis ParsePeptideHypothesis(XmlReader reader)
		{
			string peptideEvidenceId = reader.GetAttribute("peptideEvidence_ref");
			var spectrumIdentificationIds = new List<string>();
			var cvParams = new List<MzIdentMlCvParam>();
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "PeptideHypothesis")
					break;

				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "SpectrumIdentificationItemRef")
					{
						string spectrumIdentificationId = reader.GetAttribute("spectrumIdentificationItem_ref");
						spectrumIdentificationIds.Add(spectrumIdentificationId);
					}
						
					else if (reader.Name == "cvParam" || reader.Name == "userParam")
						cvParams.Add(this.GetCvParam(reader));
				}
			}

			if (string.IsNullOrEmpty(peptideEvidenceId) || !spectrumIdentificationIds.Any())
				throw new Exception("PeptideHypothesis elements require a peptideEvidenceId and spectrumIdentificationIds");

			var peptideHypothesis = new MzIdentMlPeptideHypothesis(peptideEvidenceId, spectrumIdentificationIds);

			if (cvParams.Any())
				peptideHypothesis.CvParams = cvParams;

			return peptideHypothesis;
		}
		#endregion
		#region Parameters

		private MzIdentMlCvOrUserParam GetCvOrUserParam(XmlReader reader, string elementName)
		{
			MzIdentMlCvParam? cvParam = null;
			MzIdentMlUserParam? userParam = null;

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == elementName)
					break;

				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "cvParam")
					{
						cvParam = this.GetCvParam(reader);
						break;
					}

					else if (reader.Name == "userParam")
					{
						userParam = this.GetUserParam(reader);
						break;
					}
				}
			}

			MzIdentMlCvOrUserParam cvUserParam;
			if (cvParam != null)
				cvUserParam = new MzIdentMlCvOrUserParam(cvParam);

			else if (userParam != null)
				cvUserParam = new MzIdentMlCvOrUserParam(userParam);

			else
				throw new Exception($"{elementName} must have a cvParam or userParam");

			return cvUserParam;
		}

		private MzIdentMlParamCollection GetParamCollection(XmlReader reader, string elementName)
		{
			var cvParams = new List<MzIdentMlCvParam>();
			var userParams = new List<MzIdentMlUserParam>();

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == elementName)
					break;

				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "cvParam")
						cvParams.Add(this.GetCvParam(reader));
					else if (reader.Name == "userParam")
						userParams.Add(this.GetUserParam(reader));
				}
			}

			MzIdentMlParamCollection parameters;

			bool hasCvParams = cvParams.Any();
			bool hasUserParams = userParams.Any();

			if (hasCvParams && hasUserParams)
				parameters = new MzIdentMlParamCollection(cvParams, userParams);

			else if (hasCvParams)
				parameters = new MzIdentMlParamCollection(cvParams);

			else if (hasUserParams)
				parameters = new MzIdentMlParamCollection(userParams);

			else
				throw new Exception($"{elementName} elements must have cvParams or userParams");

			return parameters;
		}

		private IEnumerable<MzIdentMlCvParam> GetNestedCvParams(XmlReader reader, string endElementName)
		{
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == endElementName)
					break;

				if (reader.NodeType == XmlNodeType.Element && (reader.Name == "cvParam"))
				{
					yield return this.GetCvParam(reader);
				}
			}
		}

		private MzIdentMlCvParam GetCvParam(XmlReader reader)
		{
			var accession = reader.GetAttribute("accession");
			var reference = reader.GetAttribute("cvRef");
			var name = reader.GetAttribute("name");

			if (accession is null || reference is null || name is null)
				throw new Exception("Error parsing cvParam");

			return new MzIdentMlCvParam(accession, reference, name)
			{
				Value = reader.GetAttribute("value"),
				UnitAccession = reader.GetAttribute("unitAccession"),
				UnitName = reader.GetAttribute("unitName")
			};
		}

		private MzIdentMlUserParam GetUserParam(XmlReader reader)
		{
			var name = reader.GetAttribute("name");
			if (name is null)
				throw new Exception($"Error parsing userParam");

			return new MzIdentMlUserParam(name)
			{
				Value = reader.GetAttribute("value"),
				UnitAccession = reader.GetAttribute("unitAccession"),
				UnitName = reader.GetAttribute("unitName"),
				Type = reader.GetAttribute("type")
			};
		}
		#endregion

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose() { }
		#endregion

		private readonly string _path;
	}
}
