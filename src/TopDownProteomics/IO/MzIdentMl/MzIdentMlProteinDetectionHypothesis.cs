using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the ProteinDetectionHypothesis element
	/// </summary>
	public class MzIdentMlProteinDetectionHypothesis
	{
		/// <summary>
		/// Instantiates with schema-required parameters
		/// </summary>
		/// <param name="id"></param>
		/// <param name="databaseSequenceId"></param>
		/// <param name="passesThreshold"></param>
		/// <param name="peptideHypotheses"></param>
		public MzIdentMlProteinDetectionHypothesis(string id, string databaseSequenceId, bool passesThreshold, List<MzIdentMlPeptideHypothesis> peptideHypotheses)
		{
			this.Id = id;
			this.DatabaseSequenceId = databaseSequenceId;
			this.PassesThreshold = passesThreshold;
			this.PeptideHypotheses = peptideHypotheses;
		}

		/// <summary>
		/// Gets the id
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Gets the database sequence ids
		/// </summary>
		public string DatabaseSequenceId { get; }

		/// <summary>
		/// Gets the passes threshold flag
		/// </summary>
		public bool PassesThreshold { get; }

		/// <summary>
		/// Gets the peptide hypotheses
		/// </summary>
		public List<MzIdentMlPeptideHypothesis> PeptideHypotheses { get; }

		/// <summary>
		/// Gets and sets the cvParams
		/// </summary>
		public List<MzIdentMlCvParam>? CvParams { get; set; }

		/// <summary>
		/// Gets and sets the userParams
		/// </summary>
		public List<MzIdentMlUserParam>? UserParams { get; set; }
	}
}
