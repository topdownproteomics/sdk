using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the PeptideEvidence element
	/// </summary>
	public class MzIdentMlPeptideEvidence
    {
        /// <summary>
        /// Instantiates with schema-required parameters
        /// </summary>
        /// <param name="databaseSequenceId"></param>
        /// <param name="id"></param>
        /// <param name="peptideId"></param>
		public MzIdentMlPeptideEvidence(string databaseSequenceId, string id, string peptideId)
		{
            this.DatabaseSequenceId = databaseSequenceId;
            this.Id = id;
            this.PeptideId = peptideId;
		}

        /// <summary>
        /// Gets and sets the database sequence id
        /// </summary>
        public string DatabaseSequenceId { get; }

        /// <summary>
        /// Gets and sets the peptide id
        /// </summary>
        public string PeptideId { get; }

        /// <summary>
        /// Gets and sets the id
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets and sets the start location
        /// </summary>
        public int? Start { get; set; }

        /// <summary>
        /// Gets and sets the end location
        /// </summary>
        public int? End { get; set; }

        /// <summary>
        /// Gets and sets the pre
        /// </summary>
        public string? Pre { get; set; }

        /// <summary>
        /// Gets and sets the post
        /// </summary>
        public string? Post { get; set; }

        /// <summary>
        /// Gets and sets the is decoy flag
        /// </summary>
        public bool? IsDecoy { get; set; }

        /// <summary>
        /// Gets and sets the cvParams
        /// </summary>
		public List<MzIdentMlCvParam>? CvParams { get; set; }

        /// <summary>
        /// Gets and sets the cvParams
        /// </summary>
		public List<MzIdentMlUserParam>? UserParams { get; set; }
    }
}
