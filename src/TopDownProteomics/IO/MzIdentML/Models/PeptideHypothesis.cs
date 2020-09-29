using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.IO.MzIdentML.Models
{
	/// <summary>
	/// Corresponds to the PeptideHypothesis element
	/// </summary>
	public class PeptideHypothesis
	{
		/// <summary>
		/// Gets and sets the PeptideEvidence ids
		/// </summary>
		public string PeptideEvidenceId { get; set; }

		/// <summary>
		/// Gets and sets the SpectrumIdentification ids
		/// </summary>
		public string SpectrumIdentificationItemId { get; set; }

		/// <summary>
		/// Gets and sets the cvParams
		/// </summary>
		public List<CvParam> CvParams { get; set; } = new List<CvParam>();
	}
}
