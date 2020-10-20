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
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Peptide")
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

			string sequence = "";
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
				databaseSequence.CvParams = cvParams;

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
			// for looking up database sequences by ID
			Dictionary<string, MzIdentMlDatabaseSequence> databaseSequences = this.GetDatabaseSequences()
				.Select(x => new { x.Id, x })
				.ToDictionary(x => x.Id, x => x.x);

			// for looking up peptide evidences by peptide ID
			var evidences = new Dictionary<string, List<MzIdentMlPeptideEvidence>>();
			foreach (var evidence in this.GetPeptideEvidences())
			{
				if (!evidences.ContainsKey(evidence.PeptideId))
					evidences[evidence.PeptideId] = new List<MzIdentMlPeptideEvidence>();

				evidences[evidence.PeptideId].Add(evidence);
			}
			
			using var fs = File.OpenRead(this._path);
			using var reader = XmlReader.Create(fs);
			foreach (var peptide in this.GetPeptidesWithoutDbSequence(reader))
			{
				bool hasEvidence = evidences.TryGetValue(peptide.Id, out var peptideEvidences);
				if (hasEvidence == false)
				{
					throw new Exception("PeptideEvidence elements must contain a matching peptide and database sequence");
				}

				foreach (var peptideEvidence in peptideEvidences)
				{
					bool hasDatabaseSequence = databaseSequences.TryGetValue(peptideEvidence.DatabaseSequenceId, out var databaseSequence);
					if (hasDatabaseSequence == false)
					{
						throw new Exception("PeptideEvidence elements must contain a matching peptide and database sequence");
					}

					peptide.DatabaseSequences ??= new List<MzIdentMlDatabaseSequence>();
					peptide.DatabaseSequences.Add(databaseSequence);

					peptide.PeptideEvidences ??= new List<MzIdentMlPeptideEvidence>();
					peptide.PeptideEvidences.Add(peptideEvidence);
				}

				yield return peptide;
			}
		}

		private IEnumerable<MzIdentMlPeptide> GetPeptidesWithoutDbSequence(XmlReader reader)
		{
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "SequenceCollection")
					break;

				if (reader.NodeType == XmlNodeType.Element && reader.Name == "Peptide")
				{
					yield return this.ParsePeptide(reader);
				}
			}
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
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "SequenceCollection")
					break;

				if (reader.NodeType == XmlNodeType.Element && reader.Name == "PeptideEvidence")
				{
					yield return this.ParsePeptideEvidences(reader);
				}
			}
		}

		private MzIdentMlPeptideEvidence ParsePeptideEvidences(XmlReader reader)
		{
			var databaseSequenceid = reader.GetAttribute("dBSequence_ref");
			var id = reader.GetAttribute("id");
			var peptideId = reader.GetAttribute("peptide_ref");

			if (string.IsNullOrEmpty(databaseSequenceid) || string.IsNullOrEmpty(id) || string.IsNullOrEmpty(peptideId))
				throw new Exception("PeptideEvidence elements must have an id, database sequence reference, and peptide reference");

			var peptideEvidence = new MzIdentMlPeptideEvidence(databaseSequenceid, id, peptideId)
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
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "ProteinDetectionProtocol")
					break;

				if (reader.NodeType == XmlNodeType.Element && reader.Name == "SpectrumIdentificationProtocol")
				{
					yield return this.ParseSpectrumIdentificationProtocol(reader);
				}
			}
		}

		private MzIdentMlSpectrumIdentificationProtocol ParseSpectrumIdentificationProtocol(XmlReader reader)
		{
			var id = reader.GetAttribute("id");
			var softwareId = reader.GetAttribute("analysisSoftware_ref");

			MzIdentMlCvOrUserParam? searchType = null;

			MzIdentMlParamCollection? thresholds = null;
			MzIdentMlParamCollection? searchParams = null;
			MzIdentMlParamCollection? databaseFilterParams = null;
			MzIdentMlParamCollection? fragmentTolerances = null;
			MzIdentMlParamCollection? precursorTolerances = null;

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "ProteinDetectionProtocol")
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

			var protocol = new MzIdentMlSpectrumIdentificationProtocol(id, softwareId, searchType, thresholds)
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
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "AnalysisProtocolCollection")
					break;

				if (reader.NodeType == XmlNodeType.Element && reader.Name == "ProteinDetectionProtocol")
				{
					yield return this.ParseProteinDetectionProtocol(reader);
				}
			}
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
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "AnalysisProtocolCollection")
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
			var sourceFiles = new List<MzIdentMlSourceFile>();
			var searchDatabases = new List<MzIdentMlSearchDatabase>();
			MzIdentMlSpectraData? spectraData = null;

			using Stream fs = File.OpenRead(this._path);
			using XmlReader reader = XmlReader.Create(fs);
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

			if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(location) || string.IsNullOrEmpty(name) || fileFormat is null || databaseName is null)
				throw new Exception("SearchDatabase elements must contain an id, location, name, fileFormat, and database name");

			var database = new MzIdentMlSearchDatabase(id, location, fileFormat, databaseName)
			{
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
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "FragmentationTable")
					break;

				if (reader.NodeType == XmlNodeType.Element && reader.Name == "Measure")
				{
					var id = reader.GetAttribute("id");

					// only one cvParam per spec doc
					var cvParam = this.GetNestedCvParams(reader, "FragmentationTable").First();

					yield return new MzIdentMlFragmentationMeasure(id, cvParam);
				}
			}
		}

		/// <summary>
		/// Gets spectral identifications with related peptides and database sequences
		/// </summary>
		/// <returns></returns>
		public IEnumerable<MzIdentMlSpectrumIdentificationItem> GetSpectrumIdentificationItems()
		{
			// create a hash table for looking up peptides by PeptideEvidence IDs
			var peptidesHash = new Dictionary<string, MzIdentMlPeptide>();
			foreach (MzIdentMlPeptide peptide in this.GetPeptides())
			{
				if (peptide.PeptideEvidences != null)
				{
					foreach (var evidence in peptide.PeptideEvidences)
					{
						peptidesHash[evidence.Id] = peptide;
					}
				}
			}

			// use PeptideEvidence to find matching peptides for each PSM
			using Stream fs = File.OpenRead(this._path);
			using var reader = XmlReader.Create(fs);
			foreach (var result in this.GetSpectrumIdentificationResults(reader))
			{
				foreach (var id in result.SpectrumIdentificationItems)
				{
					foreach (var evidenceId in id.PeptideEvidenceIds)
					{
						bool hasPeptide = peptidesHash.TryGetValue(evidenceId, out MzIdentMlPeptide peptide);

						if (hasPeptide && peptide.PeptideEvidences != null)
						{
							id.Peptides ??= new List<MzIdentMlPeptide>();
							id.PeptideEvidences ??= new List<MzIdentMlPeptideEvidence>();

							id.Peptides.Add(peptide);
							id.PeptideEvidences.Add(peptide.PeptideEvidences.Find(x => x.Id == evidenceId));
						}

						else
							throw new Exception($"Unable to find matching peptide for SpectrumIdentificationItem {id.Id}");
					}
					yield return id;
				}
			}
		}

		private IEnumerable<MzIdentMlSpectrumIdentificationResult> GetSpectrumIdentificationResults(XmlReader reader)
		{
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "SpectrumIdentificationList")
					break;

				if (reader.NodeType == XmlNodeType.Element && reader.Name == "SpectrumIdentificationResult")
				{
					yield return this.ParseSpectrumIdentificationResult(reader);
				}
			}
		}

		private MzIdentMlSpectrumIdentificationResult ParseSpectrumIdentificationResult(XmlReader reader)
		{
			var id = reader.GetAttribute("id");
			var spectrumId = reader.GetAttribute("spectrumID");
			var inputSpectraDataId = reader.GetAttribute("spectraData_ref");

			var spectrumIdentificationItems = new List<MzIdentMlSpectrumIdentificationItem>();
			var cvParams = new List<MzIdentMlCvParam>();
			var userParams = new List<MzIdentMlUserParam>();

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "SpectrumIdentificationList")
					break;

				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "SpectrumIdentificationItem")
						spectrumIdentificationItems.Add(this.ParseSpectrumIdentificationItem(reader));

					else if (reader.Name == "cvParam")
						cvParams.Add(this.GetCvParam(reader));

					else if (reader.Name == "userParam")
						userParams.Add(this.GetUserParam(reader));
				}
			}

			if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(spectrumId) || string.IsNullOrEmpty(inputSpectraDataId) || spectrumIdentificationItems.Count == 0)
				throw new Exception("SpectrumIdentificationResult elements require SpectrumIdentificationItems and an id, spectrumID, and spectraData_ref.");

			var spectrumIdentificationResult = new MzIdentMlSpectrumIdentificationResult(id, spectrumId, inputSpectraDataId, spectrumIdentificationItems);

			if (cvParams.Any())
				spectrumIdentificationResult.CvParams = cvParams;

			if (userParams.Any())
				spectrumIdentificationResult.UserParams = userParams;

			return spectrumIdentificationResult;
		}

		private MzIdentMlSpectrumIdentificationItem ParseSpectrumIdentificationItem(XmlReader reader)
		{
			var id = reader.GetAttribute("id");
			var calculatedMzAttribute = reader.GetAttribute("calculatedMassToCharge");
			var chargeStateAttribute = reader.GetAttribute("chargeState");
			var experimentalMzAttribute = reader.GetAttribute("experimentalMassToCharge");
			var rankAttribute = reader.GetAttribute("rank");
			var passesThresholdAttribute = reader.GetAttribute("passThreshold");

			var peptideEvidenceIds = new List<string>();
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
						peptideEvidenceIds.Add(reader.GetAttribute("peptideEvidence_ref"));

					else if (reader.Name == "IonType")
						ionTypes.Add(this.ParseIonType(reader));

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
				peptideEvidenceIds.Count() == 0)

				throw new Exception("SpectrumIdentificationItem elements must contain PeptideEvidenceRefs and an id, calculatedMz, chargeState, experimentalMz, rank, and passesThreshold");

			var spectrumId = new MzIdentMlSpectrumIdentificationItem(id, chargeState, calculatedMz, experimentalMz, passesThreshold, rank, peptideEvidenceIds);

			if (ionTypes.Any())
				spectrumId.IonTypes = ionTypes;

			if (cvParams.Any())
				spectrumId.CvParams = cvParams;

			if (userParams.Any())
				spectrumId.UserParams = userParams;

			return spectrumId;
		}

		private MzIdentMlIonType ParseIonType(XmlReader reader)
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
						fragmentArrays.Add(this.ParseFragmentArray(reader));
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
				ionType.FragmentArrays = fragmentArrays;

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

		private MzIdentMlFragmentArray ParseFragmentArray(XmlReader reader)
		{
			double[] values = this.ParseFragmentValues(reader.GetAttribute("values"));
			var measureId = reader.GetAttribute("measure_ref");

			if (values.Length == 0 || string.IsNullOrEmpty(measureId))
				throw new Exception("FragmentArray elements must contain values and a measure_ref");


			return new MzIdentMlFragmentArray(measureId, values);
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
		public MzIdentMlProteinDetectionList? GetProteinDetectionList()
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
				}
			}

			if (proteinDetectionList == null)
				throw new Exception("ProteinDetectionList not found");

			return proteinDetectionList;
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

				if (reader.NodeType == XmlNodeType.EndElement)
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
			string peptideEvidenceId = "";
			string spectrumIdentificationId = "";
			var cvParams = new List<MzIdentMlCvParam>();

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "PeptideHypothesis")
					break;

				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "PeptideHypothesis")
						peptideEvidenceId = reader.GetAttribute("peptideEvidence_ref");
					else if (reader.Name == "SpectrumIdentificationItemRef")
						spectrumIdentificationId = reader.GetAttribute("spectrumIdentificationItem_ref");
					else if (reader.Name == "cvParam" || reader.Name == "userParam")
						cvParams.Add(this.GetCvParam(reader));
				}
			}

			if (string.IsNullOrEmpty(peptideEvidenceId) || string.IsNullOrEmpty(spectrumIdentificationId))
				throw new Exception("PeptideHypothesis elements require a peptideEvidenceId and spectrumIdentificationId");

			var peptideHypothesis = new MzIdentMlPeptideHypothesis(peptideEvidenceId, spectrumIdentificationId);

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
