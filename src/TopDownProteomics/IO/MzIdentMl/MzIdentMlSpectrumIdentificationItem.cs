using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the SpectrumIdentificationItem elements
	/// </summary>
	public class MzIdentMlSpectrumIdentificationItem
	{
		/// <summary>
		/// Instantiates with required parameters
		/// </summary>
		/// <param name="id"></param>
		/// <param name="chargeState"></param>
		/// <param name="calculatedMz"></param>
		/// <param name="experimentalMz"></param>
		/// <param name="passesThreshold"></param>
		/// <param name="rank"></param>
		/// <param name="peptideEvidenceIds"></param>
		public MzIdentMlSpectrumIdentificationItem(string id, int chargeState, double calculatedMz, double experimentalMz, bool passesThreshold, int rank, List<string> peptideEvidenceIds)
		{
			this.Id = id;
			this.ChargeState = chargeState;
			this.CalculatedMz = calculatedMz;
			this.ExperimentalMz = experimentalMz;
			this.PassesThreshold = passesThreshold;
			this.Rank = rank;
			this.PeptideEvidenceIds = peptideEvidenceIds;
		}

		/// <summary>
		/// Gets and sets the Id
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Gets and sets the calculated m/z
		/// </summary>
		public double CalculatedMz { get; set; }

		/// <summary>
		/// Gets and sets the charge state
		/// </summary>
		public int ChargeState { get; }

		/// <summary>
		/// Gets and sets the experimental m/z
		/// </summary>
		public double ExperimentalMz { get; }

		/// <summary>
		/// Gets and sets the rank
		/// </summary>
		public int Rank { get; }

		/// <summary>
		/// Gets and sets the passes threshold flag
		/// </summary>
		public bool PassesThreshold { get; }

		/// <summary>
		/// Gets and sets the peptide evidence ids
		/// </summary>
		public List<string> PeptideEvidenceIds { get; }

		/// <summary>
		/// Gets and sets the peptide evidences
		/// </summary>
		public List<MzIdentMlPeptideEvidence>? PeptideEvidences { get; set; }

		/// <summary>
		/// Gets and sets the proteoforms
		/// </summary>
		public List<MzIdentMlPeptide>? Peptides { get; set; }

		/// <summary>
		/// Gets and sets the ion types
		/// </summary>
		public List<MzIdentMlIonType>? IonTypes { get; set; }

		/// <summary>
		/// Gets and sets cv params
		/// </summary>
		public List<MzIdentMlCvParam>? CvParams { get; set; }

		/// <summary>
		/// Gets and sets user params
		/// </summary>
		public List<MzIdentMlUserParam>? UserParams { get; set; }
	}
}
