using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.IO.MzIdentML.Models
{
	/// <summary>
	/// Corresponds to the SpectrumIdentificationItem elements
	/// </summary>
	public class SpectrumIdentificationItem
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
		public List<PeptideEvidence> PeptideEvidences { get; set; } = new List<PeptideEvidence>();

		/// <summary>
		/// Gets and sets the proteoforms
		/// </summary>
		public List<Peptide> Peptides { get; set; } = new List<Peptide>();

		/// <summary>
		/// Gets and sets the ion types
		/// </summary>
		public List<IonType> IonTypes { get; set; } = new List<IonType>();

		/// <summary>
		/// Gets and sets cvParams
		/// </summary>
		public List<CvParam> CvParams { get; set; } = new List<CvParam>();
	}
}
