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
	public class MzIdentMlParser: IDisposable
    {
        private readonly Stream _stream;

        /// <summary>
        /// Instantiates MzIdentMLReader with a stream
        /// </summary>
        /// <param name="stream"></param>
        public MzIdentMlParser(Stream stream)
        {
            this._stream = stream;
        }

        /// <summary>
        /// Instantiates MzIdentMLReader with a path
        /// </summary>
        /// <param name="path"></param>
        public MzIdentMlParser(string path)
        {
            this._stream = File.OpenRead(Path.GetFullPath(path));
        }

        /// <summary>
        /// Gets analysis softwares
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MzIdentMlAnalysisSoftware> GetAnalysisSoftware()
        {
            this._stream.Seek(0, SeekOrigin.Begin);
            using (XmlReader reader = XmlReader.Create(this._stream))
            {
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
        }

        /// <summary>
        /// Gets database sequences
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MzIdentMlDatabaseSequence> GetDatabaseSequences()
        {
            this._stream.Seek(0, SeekOrigin.Begin);
            var databaseSequences = new List<MzIdentMlDatabaseSequence>();
            using (XmlReader reader = XmlReader.Create(this._stream))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "SequenceCollection")
                        break;

                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "DBSequence")
                    {
                        databaseSequences.Add(this.ParseDatabaseSequence(reader));
                        //yield return this.ParseDatabaseSequence(reader);
                    }
                }
            }
            return databaseSequences;
        }

        /// <summary>
        /// Gets peptides
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MzIdentMlPeptide> GetPeptides()
        {
            this._stream.Seek(0, SeekOrigin.Begin);

            // create hashes for looking up related peptides & databaseSequences for each spectrum ID
            var databaseSequences = this.GetDatabaseSequences().Select(x => new { x.Id, x }).ToDictionary(x => x.Id, x => x.x);
            var peptideEvidences = this.GetPeptideEvidences().Select(x => new { x.PeptideId, x }).ToDictionary(x => x.PeptideId, x => x.x);

            foreach (var peptide in this.GetPeptidesWithoutDbSequence())
            {
                var hasPeptideEvidence = peptideEvidences.TryGetValue(peptide.Id, out MzIdentMlPeptideEvidence evidence);
                var hasDatabaseSequence = databaseSequences.TryGetValue(evidence.DatabaseSequenceId, out MzIdentMlDatabaseSequence databaseSequence);

                if (hasPeptideEvidence && hasDatabaseSequence)
                {
                    peptide.DatabaseSequence = databaseSequence;
                    peptide.PeptideEvidence = evidence;
                }

                else
                    Console.WriteLine($"PeptideEvidence {evidence.Id} is improperly formatted");

                yield return peptide;
            }
        }

        private IEnumerable<MzIdentMlPeptide> GetPeptidesWithoutDbSequence()
        {
            this._stream.Seek(0, SeekOrigin.Begin);
            using (XmlReader reader = XmlReader.Create(this._stream))
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
        }

        /// <summary>
        /// Gets peptide evidences
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MzIdentMlPeptideEvidence> GetPeptideEvidences()
        {
			this._stream.Seek(0, SeekOrigin.Begin);
            using (XmlReader reader = XmlReader.Create(this._stream))
            {
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
        }

        /// <summary>
        /// Gets SpectrumIdentificationProtocols
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MzIdentMlSpectrumIdentificationProtocol> GetSpectrumIdentificationProtocols()
        {
            this._stream.Seek(0, SeekOrigin.Begin);
            using (XmlReader reader = XmlReader.Create(this._stream))
            {
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
        }

        /// <summary>
        /// Gets ProteinDetectionProtocols
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MzIdentMlProteinDetectionProtocol> GetProteinDetectionProtocols()
        {
            this._stream.Seek(0, SeekOrigin.Begin);
            using (XmlReader reader = XmlReader.Create(this._stream))
            {
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
        }

        /// <summary>
        /// Gets Inputs
        /// </summary>
        /// <returns></returns>
        public MzIdentMlInputs GetInputs()
        {
            var inputs = new MzIdentMlInputs();
            this._stream.Seek(0, SeekOrigin.Begin);
            using (XmlReader reader = XmlReader.Create(this._stream))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Inputs")
                        break;

                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.Name)
                        {
                            case "SourceFile":
                                inputs.SourceFile = this.ParseSourceFile(reader);
                                break;
                            case "SearchDatabase":
                                inputs.SearchDatabase = this.ParseSearchDatabase(reader);
                                break;
                            case "SpectraData":
                                inputs.SpectraData = this.ParseInputSpectraData(reader);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return inputs;
        }

        /// <summary>
        /// Gets FragmentationTables
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MzIdentMlFragmentationMeasure> GetFragmentationMeasures()
        {
            this._stream.Seek(0, SeekOrigin.Begin);
            using (XmlReader reader = XmlReader.Create(this._stream))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "FragmentationTable")
                        break;

                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Measure")
                    {
                        yield return new MzIdentMlFragmentationMeasure
                        {
                            Id = reader.GetAttribute("id"),
                            Measure = this.GetChildNodeParams(reader, "FragmentationTable").FirstOrDefault()
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Gets ProteinDetectionList
        /// </summary>
        /// <returns></returns>
        public MzIdentMlProteinDetectionList GetProteinDetectionList()
        {
            var proteinDetectionList = new MzIdentMlProteinDetectionList();
            this._stream.Seek(0, SeekOrigin.Begin);
            using (XmlReader reader = XmlReader.Create(this._stream))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "AnalysisData")
                        break;

                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "ProteinDetectionList")
                    {
                        proteinDetectionList.Id = reader.GetAttribute("id");
                        proteinDetectionList.ProteinAmbiguityGroups = this.ParseProteinDetectList(reader).ToList();
                    }
                }
            }

            return proteinDetectionList;
        }

        /// <summary>
        /// Gets peptide-spectral matches with related peptides
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MzIdentMlSpectrumIdentificationItem> GetSpectrumIdentificationItems()
        {
            // create hashes for looking up related peptides & databaseSequences for each spectrum ID
            var peptides = this.GetPeptides().Select(x => new { x.PeptideEvidence.Id, x }).ToDictionary(x => x.Id, x => x.x);

            foreach (var result in this.GetSpectrumIdentificationResultsWithoutSequences())
            {
                foreach (var id in result.SpectrumIdentificationItems)
                {
                    foreach (var evidenceId in id.PeptideEvidenceIds)
                    {
                        var hasPeptide = peptides.TryGetValue(evidenceId, out MzIdentMlPeptide peptide);

                        if (hasPeptide)
                        {
                            id.Peptides.Add(peptide);
                            id.PeptideEvidences.Add(peptide.PeptideEvidence);
                        }

                        else
                            Console.WriteLine($"PeptideEvidence {evidenceId} is improperly formatted");
                    }
                    yield return id;
                }
            }
        }

        private IEnumerable<MzIdentMlSpectrumIdentificationResult> GetSpectrumIdentificationResultsWithoutSequences()
        {
            this._stream.Seek(0, SeekOrigin.Begin);
            using (XmlReader reader = XmlReader.Create(this._stream))
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
        }

        private IEnumerable<MzIdentMlProteinAmbiguityGroup> ParseProteinDetectList(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "ProteinDetectionList")
                    break;

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "ProteinAmbiguityGroup")
                {
                    yield return new MzIdentMlProteinAmbiguityGroup
                    {
                        Id = reader.GetAttribute("id"),
                        ProteinDetectionHypotheses = this.ParseProteinDetectionHypotheses(reader).ToList()
                    };
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
                    yield return new MzIdentMlProteinDetectionHypothesis
                    {
                        Id = reader.GetAttribute("id"),
                        DatabaseSequenceId = reader.GetAttribute("dBSequence_ref"),
                        PassesThreshold = Convert.ToBoolean(reader.GetAttribute("passThreshold")),
                        PeptideHypothesis = this.ParsePeptideHypothesis(reader)
                    };
                }
            }
        }

        private MzIdentMlPeptideHypothesis ParsePeptideHypothesis(XmlReader reader)
        {
            var peptideHypothesis = new MzIdentMlPeptideHypothesis();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "ProteinDetectionHypothesis")
                    break;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "PeptideHypothesis")
                        peptideHypothesis.PeptideEvidenceId = reader.GetAttribute("peptideEvidence_ref");
                    else if (reader.Name == "SpectrumIdentificationItemRef")
                        peptideHypothesis.PeptideEvidenceId = reader.GetAttribute("peptideEvidence_ref");
                    else if (reader.Name == "cvParam" || reader.Name == "userParam")
                    {
                        peptideHypothesis.Parameters.Add(this.GetParam(reader));
                    }
                }
            }

            return peptideHypothesis;
        }

        private MzIdentMlSpectrumIdentificationResult ParseSpectrumIdentificationResult(XmlReader reader)
        {
            var spectrumIdResult = new MzIdentMlSpectrumIdentificationResult
            {
                Id = reader.GetAttribute("id"),
                SpectrumId = reader.GetAttribute("spectrumID"),
                InputSpectraDataId = reader.GetAttribute("spectraData_ref")
            };
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "SpectrumIdentificationList")
                    break;

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "SpectrumIdentificationItem")
                {
                    spectrumIdResult.SpectrumIdentificationItems.Add(this.ParseSpectrumIdentificationItem(reader));
                }

                if (reader.NodeType == XmlNodeType.Element && (reader.Name == "cvParam" || reader.Name == "userParam"))
                {
                    spectrumIdResult.Parameters.Add(this.GetParam(reader));
                }
            }

            return spectrumIdResult;
        }

        private MzIdentMlSpectrumIdentificationItem ParseSpectrumIdentificationItem(XmlReader reader)
        {
            var spectrumIdItem = new MzIdentMlSpectrumIdentificationItem
            {
                Id = reader.GetAttribute("id"),
                CalculatedMz = Convert.ToDouble(reader.GetAttribute("calculatedMassToCharge")),
                ChargeState = Convert.ToInt32(reader.GetAttribute("chargeState")),
                ExperimentalMz = Convert.ToDouble(reader.GetAttribute("experimentalMassToCharge")),
                Rank = Convert.ToInt32(reader.GetAttribute("rank")),
                PassesThreshold = Convert.ToBoolean(reader.GetAttribute("passThreshold")),
            };
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "SpectrumIdentificationItem")
                    break;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "PeptideEvidenceRef")
                        spectrumIdItem.PeptideEvidenceIds.Add(reader.GetAttribute("peptideEvidence_ref"));
                    else if (reader.Name == "IonType")
                        spectrumIdItem.IonTypes.Add(this.ParseIonType(reader));
                    else if (reader.Name == "cvParam" || reader.Name == "userParam")
                    {
                        spectrumIdItem.Parameters.Add(this.GetParam(reader));
                    }
                }
            }

            return spectrumIdItem;
        }

        private MzIdentMlIonType ParseIonType(XmlReader reader)
        {
            int[] indices = reader.GetAttribute("index").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x)).ToArray();
            var ionType = new MzIdentMlIonType()
            {
                Indices = new int[indices.Length],
                Charge = Convert.ToInt32(reader.GetAttribute("charge"))
            };

            ionType.Indices = indices;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "IonType")
                    break;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "FragmentArray")
                    {
                        switch (reader.GetAttribute("measure_ref"))
                        {
                            case "m_mz":
                                ionType.Mzs = this.ConvertStringToDoubleArray(reader.GetAttribute("values"));
                                break;
                            case "m_intensity":
                                ionType.Intensities = this.ConvertStringToDoubleArray(reader.GetAttribute("values"));
                                break;
                            case "m_error":
                                ionType.MzError = this.ConvertStringToDoubleArray(reader.GetAttribute("values"));
                                break;
                            default:
                                break;
                        }
                    }

                    else if (reader.Name == "cvParam")
                    {
                        ionType.Parameters.Add(this.GetParam(reader));
                    }
                }
            }

            return ionType;
        }

        private double[] ConvertStringToDoubleArray(string stringArray, char delimiter = ' ')
        {
            var test = stringArray.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
            return stringArray.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToDouble(x)).ToArray();
        }

        private MzIdentMlSpectraData ParseInputSpectraData(XmlReader reader)
		{
            var spectraData = new MzIdentMlSpectraData
            {
                Id = reader.GetAttribute("id"),
                Location = reader.GetAttribute("location")
            };
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "SpectraData")
                    break;

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "FileFormat")
                {
                    spectraData.FileFormat = this.GetChildNodeParams(reader, "FileFormat").First();
                }
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "SpectrumIDFormat")
                {
                    spectraData.SpectrumIdFormat = this.GetChildNodeParams(reader, "SpectrumIDFormat").First();
                }
            }

            return spectraData;
        }

		private MzIdentMlSearchDatabase ParseSearchDatabase(XmlReader reader)
		{
            var inputDatabase = new MzIdentMlSearchDatabase
            {
                Id = reader.GetAttribute("id"),
                Location = reader.GetAttribute("location"),
                Name = reader.GetAttribute("name"),
            };
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "DatabaseName")
                    break;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "FileFormat")
                    {
                        inputDatabase.FileFormat = this.GetChildNodeParams(reader, "FileFormat").First();
                    }

                    else if (reader.Name == "cvParam" || reader.Name == "userParam")
                    {
                        inputDatabase.DatabaseParams.Add(this.GetParam(reader));
                    }
                }   
            }

            return inputDatabase;
        }

		private MzIdentMlSourceFile ParseSourceFile(XmlReader reader)
		{
            var sourceFile = new MzIdentMlSourceFile
            {
                Id = reader.GetAttribute("id"),
                Location = reader.GetAttribute("location")
            };
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "SourceFile")
                    break;

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "FileFormat")
                {
                    sourceFile.FileFormat = this.GetChildNodeParams(reader, "FileFormat").First();
                }
            }

            return sourceFile;
        }

        private MzIdentMlProteinDetectionProtocol ParseProteinDetectionProtocol(XmlReader reader)
		{
            var protocol = new MzIdentMlProteinDetectionProtocol
            {
                Id = reader.GetAttribute("id"),
                SoftwareId = reader.GetAttribute("analysisSoftware_ref")
            };
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "AnalysisProtocolCollection")
                    break;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case ("AnalysisParams"):
                            protocol.AnalysisParams = this.GetChildNodeParams(reader, reader.Name).ToList();
                            break;
                        case ("Threshold"):
                            protocol.Thresholds = this.GetChildNodeParams(reader, reader.Name).ToList();
                            break;
                        default:
                            break;
                    }
                }
            }

            return protocol;
        }

		private MzIdentMlSpectrumIdentificationProtocol ParseSpectrumIdentificationProtocol(XmlReader reader)
        {
            var protocol = new MzIdentMlSpectrumIdentificationProtocol
            {
                Id = reader.GetAttribute("id"),
                SoftwareId = reader.GetAttribute("analysisSoftware_ref")
            };
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "ProteinDetectionProtocol")
                    break;
                
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case ("SearchType"):
                            protocol.SearchTypes = this.GetChildNodeParams(reader, reader.Name).ToList();
                            break;
                        case ("AdditionalSearchParams"):
                            protocol.SearchParams = this.GetChildNodeParams(reader, reader.Name).ToList();
                            break;
						case ("FragmentTolerance"):
							protocol.FragmentTolerances = this.GetChildNodeParams(reader, reader.Name).ToList();
							break;
						case ("ParentTolerance"):
							protocol.PrecursorTolerances = this.GetChildNodeParams(reader, reader.Name).ToList();
							break;
						case ("Threshold"):
                            protocol.Thresholds = this.GetChildNodeParams(reader, reader.Name).ToList();
                            break;
                        case ("DatabaseFilters"):
                            protocol.DatabaseFilterParams = this.GetChildNodeParams(reader, reader.Name).ToList();
                            break;
                        default:
                            break;
					}
                }
            }

            return protocol;
        }

        private IEnumerable<MzIdentMlParam> GetChildNodeParams(XmlReader reader, string endElementName)
        {
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == endElementName)
					break;

				if (reader.NodeType == XmlNodeType.Element && (reader.Name == "cvParam" || reader.Name == "userParam"))
				{
                    yield return this.GetParam(reader);
				}
			}
        }

        private MzIdentMlParam GetParam(XmlReader reader)
        {
            return new MzIdentMlParam
            {
                Name = reader.GetAttribute("name"),
                Value = reader.GetAttribute("value"),
                Accession = reader.GetAttribute("accession"),
                UnitAccession = reader.GetAttribute("unitAccession"),
                UnitName = reader.GetAttribute("unitName")
            };
        }

        private MzIdentMlPeptideEvidence ParsePeptideEvidences(XmlReader reader)
        {
            return new MzIdentMlPeptideEvidence
            {
                Id = reader.GetAttribute("id"),
                Start = Convert.ToInt32(reader.GetAttribute("start")),
                End = Convert.ToInt32(reader.GetAttribute("end")),
                Pre = reader.GetAttribute("pre"),
                Post = reader.GetAttribute("post"),
                IsDecoy = Convert.ToBoolean(reader.GetAttribute("isDecoy")),
                DatabaseSequenceId = reader.GetAttribute("dBSequence_ref"),
                PeptideId = reader.GetAttribute("peptide_ref")
            };
        }

        private MzIdentMlPeptide ParsePeptide(XmlReader reader)
        {
            var peptide = new MzIdentMlPeptide
            {
                Id = reader.GetAttribute("id")
            };
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Peptide")
                    break;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "PeptideSequence")
                        peptide.Sequence = reader.ReadInnerXml();
                    else if (reader.Name == "Modification")
                        peptide.Modifications.Add(this.ParseModification(reader));
                }
            }

            return peptide;
        }

        private MzIdentMlModification ParseModification(XmlReader reader)
        {
            var modification = new MzIdentMlModification
            {
                Location = Convert.ToInt32(reader.GetAttribute("location")),
                MonoisotopicMassDelta = Convert.ToDouble(reader.GetAttribute("monoisotopicMassDelta")),
            };
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Modification")
                    break;

                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "cvParam")
                {
                    modification.Accession = reader.GetAttribute("accession");
                    modification.Name = reader.GetAttribute("name");
                    modification.Reference = reader.GetAttribute("cvRef");
                }
            }

            return modification;
        }

        private MzIdentMlDatabaseSequence ParseDatabaseSequence(XmlReader reader)
        {
            var sequence = new MzIdentMlDatabaseSequence
            {
                Id = reader.GetAttribute("id"),
                Length = Convert.ToInt32(reader.GetAttribute("length")),
                SearchDatabaseId = reader.GetAttribute("searchDatabase_ref"),
                Accession = reader.GetAttribute("accession")
            };

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "DBSequence")
                    break;

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Seq")
                {
                    sequence.Sequence = reader.ReadInnerXml();
                }
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "cvParam")
                {
                    string accession = reader.GetAttribute("accession");
                    string value = reader.GetAttribute("value");

                    switch (accession)
                    {
                        case "MS:1001088":
                            sequence.ProteinDescription = value;
                            break;
                        case "MS:1001469":
                            sequence.TaxonomyScientificName = value;
                            break;
                        case "MS:1001467":
                            sequence.TaxonomyId = Convert.ToInt32(value);
                            break;
                        default:
                            break;
                    }
                }
            }

            return sequence;
        }

        private MzIdentMlAnalysisSoftware ParseAnalysisSoftware(XmlReader reader)
        {
            return new MzIdentMlAnalysisSoftware
			{
				Id = reader.GetAttribute("id"),
				Name = reader.GetAttribute("name"),
				Version = reader.GetAttribute("version"),
				Uri = reader.GetAttribute("uri")
			};
        }

        /// <summary>
        /// Closes the stream
        /// </summary>
        public void Dispose() => this._stream.Close();
    }
}
