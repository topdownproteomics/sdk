using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the PeptideHypothesis element
	/// </summary>
	public class MzIdentMlPeptideHypothesis
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
		/// Gets and sets the parameters
		/// </summary>
		public List<MzIdentMlParam> Parameters { get; set; } = new List<MzIdentMlParam>();
	}
}
