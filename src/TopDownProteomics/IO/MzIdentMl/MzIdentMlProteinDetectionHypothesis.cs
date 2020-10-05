namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the ProteinDetectionHypothesis element
	/// </summary>
	public class MzIdentMlProteinDetectionHypothesis
	{
		/// <summary>
		/// Gets and sets the id
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets and sets the database sequence ids
		/// </summary>
		public string DatabaseSequenceId { get; set; }

		/// <summary>
		/// Gets and sets the passes threshold flag
		/// </summary>
		public bool PassesThreshold { get; set; }

		/// <summary>
		/// Gets and sets the peptide hypothesis
		/// </summary>
		public MzIdentMlPeptideHypothesis PeptideHypothesis { get; set; }
	}
}
