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
        /// <param name="databaseSequence"></param>
        /// <param name="id"></param>
        /// <param name="peptide"></param>
		public MzIdentMlPeptideEvidence(MzIdentMlDatabaseSequence databaseSequence, string id, MzIdentMlPeptide peptide)
		{
            this.DatabaseSequence = databaseSequence;
            this.Id = id;
            this.Peptide = peptide;
		}

        /// <summary>
        /// Gets the database sequence
        /// </summary>
        public MzIdentMlDatabaseSequence DatabaseSequence { get; }

        /// <summary>
        /// Gets  the peptide
        /// </summary>
        public MzIdentMlPeptide Peptide { get; }

        /// <summary>
        /// Gets the id
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
