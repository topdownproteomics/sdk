using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the PeptideHypothesis element
	/// </summary>
	public class MzIdentMlPeptideHypothesis
	{
		/// <summary>
		/// Instantiates with schema-required parameters
		/// </summary>
		/// <param name="peptideEvidenceId"></param>
		/// <param name="spectrumIdentificationItemId"></param>
		public MzIdentMlPeptideHypothesis(string peptideEvidenceId, string spectrumIdentificationItemId)
		{
			this.PeptideEvidenceId = peptideEvidenceId;
			this.SpectrumIdentificationItemId = spectrumIdentificationItemId;
		}

		/// <summary>
		/// Gets the PeptideEvidence ids
		/// </summary>
		public string PeptideEvidenceId { get; }

		/// <summary>
		/// Gets the SpectrumIdentification ids
		/// </summary>
		public string SpectrumIdentificationItemId { get; }

		/// <summary>
		/// Gets and sets the cvParams
		/// </summary>
		public List<MzIdentMlCvParam>? CvParams { get; set; }
	}
}
