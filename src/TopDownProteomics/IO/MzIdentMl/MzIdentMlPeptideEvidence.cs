namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the PeptideEvidence element
	/// </summary>
	public class MzIdentMlPeptideEvidence
    {
        /// <summary>
        /// Gets and sets the id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets and sets the start location
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Gets and sets the end location
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// Gets and sets the pre
        /// </summary>
        public string Pre { get; set; }

        /// <summary>
        /// Gets and sets the post
        /// </summary>
        public string Post { get; set; }

        /// <summary>
        /// Gets and sets the is decoy flag
        /// </summary>
        public bool IsDecoy { get; set; }

        /// <summary>
        /// Gets and sets the database sequence id
        /// </summary>
        public string DatabaseSequenceId { get; set; }

        /// <summary>
        /// Gets and sets the peptide id
        /// </summary>
        public string PeptideId { get; set; }
    }
}
