using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the Peptide element
	/// </summary>
	public class MzIdentMlPeptide
    {
		/// <summary>
		/// Instantiates with required parameters
		/// </summary>
		/// <param name="id"></param>
		/// <param name="sequence"></param>
		public MzIdentMlPeptide(string id, string sequence)
		{
            this.Id = id;
            this.Sequence = sequence;
		}

        /// <summary>
        /// Gets the id
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the sequence
        /// </summary>
        public string Sequence { get; }

        /// <summary>
        /// Gets and sets the database sequences
        /// </summary>
        public List<MzIdentMlDatabaseSequence>? DatabaseSequences { get; set; }

        /// <summary>
        /// Gets and sets the peptide evidences
        /// </summary>
        public List<MzIdentMlPeptideEvidence>? PeptideEvidences { get; set; }

        /// <summary>
        /// Gets and sets the modifications
        /// </summary>
        public List<MzIdentMlModification>? Modifications { get; set; }

        /// <summary>
        /// Gets and sets the cvParams
        /// </summary>
		public List<MzIdentMlCvParam>? CvParams { get; set; }

        /// <summary>
        /// Gets and sets the user params
        /// </summary>
        public List<MzIdentMlUserParam>? UserParams { get; set; }
    }
}
