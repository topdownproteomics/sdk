using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.IO.MzIdentML.Models
{
	/// <summary>
	/// Corresponds to the ProteinDetectionProtocol element
	/// </summary>
	public class ProteinDetectionProtocol
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
		public List<CvParam> AnalysisParams { get; set; }

		/// <summary>
		/// Gets and sets the thresholds
		/// </summary>
		public List<CvParam> Thresholds { get; set; }
	}
}
