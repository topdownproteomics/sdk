using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the SpectrumIdentificationItem elements
	/// </summary>
	public class MzIdentMlSpectrumIdentificationItem
	{
		/// <summary>
		/// Gets and sets the Id
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets and sets the calculated m/z
		/// </summary>
		public double CalculatedMz { get; set; }

		/// <summary>
		/// Gets and sets the charge state
		/// </summary>
		public int ChargeState { get; set; }

		/// <summary>
		/// Gets and sets the experimental m/z
		/// </summary>
		public double ExperimentalMz { get; set; }

		/// <summary>
		/// Gets and sets the rank
		/// </summary>
		public int Rank { get; set; }

		/// <summary>
		/// Gets and sets the passes threshold flag
		/// </summary>
		public bool PassesThreshold { get; set; }

		/// <summary>
		/// Gets and sets the peptide evidence ids
		/// </summary>
		public List<string> PeptideEvidenceIds { get; set; } = new List<string>();

		/// <summary>
		/// Gets and sets the peptide evidences
		/// </summary>
		public List<MzIdentMlPeptideEvidence> PeptideEvidences { get; set; } = new List<MzIdentMlPeptideEvidence>();

		/// <summary>
		/// Gets and sets the proteoforms
		/// </summary>
		public List<MzIdentMlPeptide> Peptides { get; set; } = new List<MzIdentMlPeptide>();

		/// <summary>
		/// Gets and sets the ion types
		/// </summary>
		public List<MzIdentMlIonType> IonTypes { get; set; } = new List<MzIdentMlIonType>();

		/// <summary>
		/// Gets and sets parameters
		/// </summary>
		public List<MzIdentMlParam> Parameters { get; set; } = new List<MzIdentMlParam>();
	}
}
