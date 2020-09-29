using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using TopDownProteomics.IO.MzIdentML.Models;

namespace TopDownProteomics.IO.MzIdentML
{
    /// <summary>
    /// 
    /// </summary>
    public class MzIdentMLReader: IDisposable
    {
        private readonly Stream _stream;

        /// <summary>
        /// Instantiates MzIdentMLReader with a Stream
        /// </summary>
        /// <param name="stream"></param>
        public MzIdentMLReader(Stream stream)
        {
            this._stream = stream;
        }

        /// <summary>
        /// Instantiates MzIdentMLReader with a path
        /// </summary>
        /// <param name="path"></param>
        public MzIdentMLReader(string path)
        {
            this._stream = File.OpenRead(Path.GetFullPath(@"D:\mzid_samples\golden.mzid"));
        }

        /// <summary>
        /// Gets analysis softwares
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AnalysisSoftware> GetAnalysisSoftware()
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
        public IEnumerable<DatabaseSequence> GetDatabaseSequenceCollection()
        {
            this._stream.Seek(0, SeekOrigin.Begin);
            var databaseSequences = new List<DatabaseSequence>();
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
        /// Gets proteoforms
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Peptide> GetProteoforms()
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
                        yield return this.ParseProteoform(reader);
                    }
                }
            }
        }

        /// <summary>
        /// Gets ProteoformEvidences
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PeptideEvidence> GetProteoformEvidences()
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
                        yield return this.ParseProteoformEvidence(reader);
                    }
                }
            }
        }

        /// <summary>
        /// Gets SpectrumIdentificationProtocols
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SpectrumIdentificationProtocol> GetSpectrumIdentificationProtocols()
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
        public IEnumerable<ProteinDetectionProtocol> GetProteinDetectionProtocols()
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
        public Inputs GetInputs()
        {
            var inputs = new Inputs();
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
        /// Gets FragmentationTabless
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FragmentationMeasure> GetFragmentationTable()
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
                        yield return new FragmentationMeasure
                        {
                            Id = reader.GetAttribute("id"),
                            Measure = this.GetChildNodeCvParams(reader, "FragmentationTable").First()
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Gets SpectrumIdentificationResults
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SpectrumIdentificationResult> GetSpectrumIdentificationResults()
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

        /// <summary>
        /// Gets ProteinDetectionList
        /// </summary>
        /// <returns></returns>
        public ProteinDetectionList GetProteinDetectionList()
        {
            var proteinDetectionList = new ProteinDetectionList();
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
        /// Gets SpectrumIdentificationResults with related ProteoformEvidences, DatabaseSequences, and Proteoforms
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SpectrumIdentificationItem> GetFullSpectrumIdentificationResults()
        {
            var proteoformEvidences = this.GetProteoformEvidences().Select(x => new { x.Id, x }).ToDictionary(x => x.Id, x => x.x);
            var databaseSequences = this.GetDatabaseSequenceCollection().Select(x => new { x.Id, x }).ToDictionary(x => x.Id, x => x.x);
            var proteoforms = this.GetProteoforms().ToList().Select(x => new { x.Id, x }).ToDictionary(x => x.Id, x => x.x);

            foreach (var result in this.GetSpectrumIdentificationResults())
            {
                foreach (var id in result.SpectrumIdentificationItems)
                {
                    foreach (var evidence in id.PeptideEvidenceIds)
                    {
                        proteoformEvidences.TryGetValue(evidence, out PeptideEvidence proteoformEvidence);
                        proteoforms.TryGetValue(proteoformEvidence.PeptideId, out Peptide proteoform);
                        databaseSequences.TryGetValue(proteoformEvidence.DatabaseSequenceId, out DatabaseSequence databaseSequence);
                        proteoformEvidence.DatabaseSequence = databaseSequence;
                        id.Peptides.Add(proteoform);
                        id.PeptideEvidences.Add(proteoformEvidence);
                    }
                    yield return id;
                }
            }
        }

        private IEnumerable<ProteinAmbiguityGroup> ParseProteinDetectList(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "ProteinDetectionList")
                    break;

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "ProteinAmbiguityGroup")
                {
                    yield return new ProteinAmbiguityGroup
                    {
                        Id = reader.GetAttribute("id"),
                        ProteinDetectionHypotheses = this.ParseProteinDetectionHypotheses(reader).ToList()
                    };
                }
            }
        }

        private IEnumerable<ProteinDetectionHypothesis> ParseProteinDetectionHypotheses(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "ProteinAmbiguityGroup")
                    break;

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "ProteinDetectionHypothesis")
                {
                    yield return new ProteinDetectionHypothesis
                    {
                        Id = reader.GetAttribute("id"),
                        DatabaseSequenceId = reader.GetAttribute("dBSequence_ref"),
                        PassesThreshold = Convert.ToBoolean(reader.GetAttribute("passThreshold")),
                        PeptideHypothesis = this.ParsePeptideHypothesis(reader)
                    };
                }
            }
        }

        private PeptideHypothesis ParsePeptideHypothesis(XmlReader reader)
        {
            var peptideHypothesis = new PeptideHypothesis();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "ProteinDetectionHypothesis")
                    break;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "PeptideHypothesis":
                            peptideHypothesis.PeptideEvidenceId = reader.GetAttribute("peptideEvidence_ref");
                            break;
                        case "SpectrumIdentificationItemRef":
                            peptideHypothesis.SpectrumIdentificationItemId = reader.GetAttribute("spectrumIdentificationItem_ref");
                            break;
                        case "cvParam":
                            var cvParam = new CvParam();
                            this.AddCvParam(cvParam, reader);
                            peptideHypothesis.CvParams.Add(cvParam);
                            break;
                        default:
                            break;
                    }
                }
            }

            return peptideHypothesis;
        }

        private SpectrumIdentificationResult ParseSpectrumIdentificationResult(XmlReader reader)
        {
            var spectrumIdResult = new SpectrumIdentificationResult
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

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "cvParam")
                {
                    var cvParam = new CvParam();
                    this.AddCvParam(cvParam, reader);
                    spectrumIdResult.CvParams.Add(cvParam);
                }
            }

            return spectrumIdResult;
        }

        private SpectrumIdentificationItem ParseSpectrumIdentificationItem(XmlReader reader)
        {
            var spectrumIdItem = new SpectrumIdentificationItem
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
                    switch (reader.Name)
                    {
                        case "PeptideEvidenceRef":
                            spectrumIdItem.PeptideEvidenceIds.Add(reader.GetAttribute("peptideEvidence_ref"));
                            break;
                        case "IonType":
                            spectrumIdItem.IonTypes.Add(this.ParseIonType(reader));
                            break;
                        case "cvParam":
                            var cvParam = new CvParam();
                            this.AddCvParam(cvParam, reader);
                            spectrumIdItem.CvParams.Add(cvParam);
                            break;
                        default:
                            break;
                    }
                }
            }
            return spectrumIdItem;
        }

        private IonType ParseIonType(XmlReader reader)
        {
            int[] indices = reader.GetAttribute("index").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x)).ToArray();
            var ionType = new IonType()
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
                        var cvParam = new CvParam();
                        this.AddCvParam(cvParam, reader);
                        ionType.CvParams.Add(cvParam);
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

        private SpectraData ParseInputSpectraData(XmlReader reader)
		{
            var spectraData = new SpectraData
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
                    spectraData.FileFormat = this.GetChildNodeCvParams(reader, "FileFormat").First();
                }
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "SpectrumIDFormat")
                {
                    spectraData.SpectrumIdFormat = this.GetChildNodeCvParams(reader, "SpectrumIDFormat").First();
                }
            }
            return spectraData;
        }

		private SearchDatabase ParseSearchDatabase(XmlReader reader)
		{
            var inputDatabase = new SearchDatabase
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
                        inputDatabase.FileFormat = this.GetChildNodeCvParams(reader, "FileFormat").First();
                    }

                    else if (reader.Name == "DatabaseName")
                    {
                        inputDatabase.DatabaseName = this.GetChildNodeUserParams(reader, "DatabaseName").First();
                    }

                    else if (reader.Name == "cvParam")
                    {
                        var cvParam = new CvParam();
                        this.AddCvParam(cvParam, reader);
                        inputDatabase.DatabaseParams.Add(cvParam);
                    }
                }
                
            }
            return inputDatabase;
        }

		private SourceFile ParseSourceFile(XmlReader reader)
		{
            var sourceFile = new SourceFile
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
                    sourceFile.FileFormat = this.GetChildNodeCvParams(reader, "FileFormat").First();
                }
            }
            return sourceFile;
        }

        private ProteinDetectionProtocol ParseProteinDetectionProtocol(XmlReader reader)
		{
            var protocol = new ProteinDetectionProtocol
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
                            protocol.AnalysisParams = this.GetChildNodeCvParams(reader, reader.Name).ToList();
                            break;
                        case ("Threshold"):
                            protocol.Thresholds = this.GetChildNodeCvParams(reader, reader.Name).ToList();
                            break;
                        default:
                            break;
                    }
                }
            }

            return protocol;
        }

		private SpectrumIdentificationProtocol ParseSpectrumIdentificationProtocol(XmlReader reader)
        {
            var protocol = new SpectrumIdentificationProtocol
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
                            protocol.SearchTypes = this.GetChildNodeCvParams(reader, reader.Name).ToList();
                            break;
                        case ("AdditionalSearchParams"):
                            protocol.SearchParams = this.GetChildNodeCvParams(reader, reader.Name).ToList();
                            break;
						case ("FragmentTolerance"):
							protocol.FragmentTolerances = this.GetChildNodeCvParams(reader, reader.Name).ToList();
							break;
						case ("ParentTolerance"):
							protocol.PrecursorTolerances = this.GetChildNodeCvParams(reader, reader.Name).ToList();
							break;
						case ("Threshold"):
                            protocol.Thresholds = this.GetChildNodeCvParams(reader, reader.Name).ToList();
                            break;
                        case ("DatabaseFilters"):
                            protocol.DatabaseFilterParams = this.GetChildNodeCvParams(reader, reader.Name).ToList();
                            break;
                        default:
                            break;
					}
                }
            }

            return protocol;
        }

        private IEnumerable<CvParam> GetChildNodeCvParams(XmlReader reader, string endElementName)
        {
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == endElementName)
					break;

				if (reader.NodeType == XmlNodeType.Element && reader.Name == "cvParam")
				{
                    var cvParam = new CvParam();
                    this.AddCvParam(cvParam, reader);
                    yield return cvParam;
				}
			}
        }

        private void AddCvParam(ICvParam param, XmlReader reader)
        {
            param.Name = reader.GetAttribute("name");
            param.Value = reader.GetAttribute("value");
            param.Accession = reader.GetAttribute("accession");

            if (param is CvUnitParam)
            {
                (param as CvUnitParam).UnitAccession = reader.GetAttribute("unitAccession");
                (param as CvUnitParam).UnitName = reader.GetAttribute("unitName");
            }
        }

        private IEnumerable<UserParam> GetChildNodeUserParams(XmlReader reader, string endElementName)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == endElementName)
                    break;

                if (reader.NodeType == XmlNodeType.Element && (reader.Name == "userParam"))
                {
                    var userParam = new UserParam();
                    this.AddParam(userParam, reader);
                    yield return userParam;
                }
            }
        }

        private void AddParam(IUserParam param, XmlReader reader)
        {
            param.Name = reader.GetAttribute("name");
            param.Value = reader.GetAttribute("value");

            if (param is ICvParam)
                (param as ICvParam).Accession = reader.GetAttribute("accession");

            if (param is CvUnitParam)
            {
                (param as CvUnitParam).UnitAccession = reader.GetAttribute("unitAccession");
                (param as CvUnitParam).UnitName = reader.GetAttribute("unitName");
            }
        }

        private PeptideEvidence ParseProteoformEvidence(XmlReader reader)
        {
            return new PeptideEvidence
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

        private Peptide ParseProteoform(XmlReader reader)
        {
            var proteoform = new Peptide
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
                        proteoform.Sequence = reader.ReadInnerXml();
                    else if (reader.Name == "Modification")
                        proteoform.Modifications.Add(this.ParseModification(reader));
                }
            }

            proteoform.CreateProForma();
            return proteoform;
        }

        private Modification ParseModification(XmlReader reader)
        {
            var oneBasedLocationIndex = Convert.ToInt32(reader.GetAttribute("location"));
            var monoisotopicMassDelta = Convert.ToDouble(reader.GetAttribute("monoisotopicMassDelta"));
            var cvParams = new List<CvParam>();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Modification")
                    break;

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "cvParam")
                {
                    var cvParam = new CvParam();
                    this.AddCvParam(cvParam, reader);
                    cvParams.Add(cvParam);
                }
                    
            }
            return new Modification(oneBasedLocationIndex, monoisotopicMassDelta, cvParams);
        }

        private DatabaseSequence ParseDatabaseSequence(XmlReader reader)
        {
            var sequence = new DatabaseSequence
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

        private AnalysisSoftware ParseAnalysisSoftware(XmlReader reader)
        {
            return new AnalysisSoftware
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
