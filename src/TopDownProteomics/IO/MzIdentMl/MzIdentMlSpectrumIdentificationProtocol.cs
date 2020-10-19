namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the SpectrumIdentificationProtocol element
	/// </summary>
	public class MzIdentMlSpectrumIdentificationProtocol
    {
        /// <summary>
        /// Instantiates with required parameters
        /// </summary>
        /// <param name="id"></param>
        /// <param name="softwareId"></param>
        /// <param name="searchType"></param>
        /// <param name="thresholds"></param>
		public MzIdentMlSpectrumIdentificationProtocol(string id, string softwareId, MzIdentMlCvOrUserParam searchType, MzIdentMlParamCollection thresholds)
		{
            this.Id = id;
            this.SoftwareId = softwareId;
            this.SearchType = searchType;
            this.Thresholds = thresholds;
		}
        /// <summary>
        /// Gets and sets the id
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets and sets the software id
        /// </summary>
        public string SoftwareId { get; }

        /// <summary>
        /// Gets and sets the search type
        /// </summary>
        public MzIdentMlCvOrUserParam SearchType { get; }

        /// <summary>
        /// Gets and sets the search params
        /// </summary>
        public MzIdentMlParamCollection? SearchParams { get; set; }

        /// <summary>
        /// Gets and sets the database filter params
        /// </summary>
        public MzIdentMlParamCollection? DatabaseFilterParams { get; set; }

        /// <summary>
        /// Gets and sets the fragment tolerances
        /// </summary>
        public MzIdentMlParamCollection? FragmentTolerances { get; set; }

        /// <summary>
        /// Gets and sets the precursor tolerances
        /// </summary>
        public MzIdentMlParamCollection? PrecursorTolerances { get; set; }

        /// <summary>
        /// Gets the thresholds
        /// </summary>
        public MzIdentMlParamCollection Thresholds { get; }
    }
}
