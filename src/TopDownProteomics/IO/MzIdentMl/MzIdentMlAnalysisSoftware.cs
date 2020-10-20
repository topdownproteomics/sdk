namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to AnalysisSoftware element
	/// </summary>
	public class MzIdentMlAnalysisSoftware
    {
        /// <summary>
        /// Instantiates with required id
        /// </summary>
        /// <param name="id"></param>
		public MzIdentMlAnalysisSoftware(string id)
		{
            this.Id = id;
		}
        /// <summary>
        /// Gets and sets the Id
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets and sets the Name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets and sets the Version
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// Gets and sets the URI
        /// </summary>
        public string? Uri { get; set; }
    }
}
