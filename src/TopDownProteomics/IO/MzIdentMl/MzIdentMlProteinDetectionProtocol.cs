namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the ProteinDetectionProtocol element
	/// </summary>
	public class MzIdentMlProteinDetectionProtocol
	{
		/// <summary>
		/// Instantiates with required parameters
		/// </summary>
		/// <param name="id"></param>
		/// <param name="softwareId"></param>
		/// <param name="thresholds"></param>
		public MzIdentMlProteinDetectionProtocol(string id, string softwareId, MzIdentMlParamCollection thresholds)
		{
			this.Id = id;
			this.SoftwareId = softwareId;
			this.Thresholds = thresholds;
		}

		/// <summary>
		/// Gets the id
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Gets the software id
		/// </summary>
		public string SoftwareId { get; }

		/// <summary>
		/// Gets and sets the thresholds
		/// </summary>
		public MzIdentMlParamCollection Thresholds { get; set; }

		/// <summary>
		/// Gets and sets the name
		/// </summary>
		public string? Name { get; set; }

		/// <summary>
		/// Gets and sets the analysis parameters
		/// </summary>
		public MzIdentMlParamCollection? AnalysisParams { get; set; }
	}
}
