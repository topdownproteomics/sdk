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
		/// <param name="spectrumIdentificationItemIds"></param>
		public MzIdentMlPeptideHypothesis(string peptideEvidenceId, List<string> spectrumIdentificationItemIds)
		{
			this.PeptideEvidenceId = peptideEvidenceId;
			this.SpectrumIdentificationItemIds = spectrumIdentificationItemIds;
		}

		/// <summary>
		/// Gets the PeptideEvidence ids
		/// </summary>
		public string PeptideEvidenceId { get; }

		/// <summary>
		/// Gets the SpectrumIdentification ids
		/// </summary>
		public List<string> SpectrumIdentificationItemIds { get; }

		/// <summary>
		/// Gets and sets the cvParams
		/// </summary>
		public List<MzIdentMlCvParam>? CvParams { get; set; }
	}
}
