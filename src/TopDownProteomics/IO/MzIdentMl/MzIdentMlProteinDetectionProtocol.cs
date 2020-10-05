using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the ProteinDetectionProtocol element
	/// </summary>
	public class MzIdentMlProteinDetectionProtocol
	{
		/// <summary>
		/// Gets and sets the id
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets and sets the software id
		/// </summary>
		public string SoftwareId { get; set; }

		/// <summary>
		/// Gets and sets the analysis parameters
		/// </summary>
		public List<MzIdentMlParam> AnalysisParams { get; set; }

		/// <summary>
		/// Gets and sets the thresholds
		/// </summary>
		public List<MzIdentMlParam> Thresholds { get; set; }
	}
}
