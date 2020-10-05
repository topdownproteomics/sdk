using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
    /// <summary>
    /// Corresponds to the Peptide element
    /// </summary>
    public class MzIdentMlPeptide
    {
        /// <summary>
        /// Gets and sets the id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets and sets the sequence
        /// </summary>
        public string Sequence { get; set; }

        /// <summary>
        /// Gets and sets the modifications
        /// </summary>
        public List<MzIdentMlModification> Modifications { get; set; } = new List<MzIdentMlModification>();

        /// <summary>
        /// gets and sets the database sequence
        /// </summary>
        public MzIdentMlDatabaseSequence DatabaseSequence { get; set; }

        /// <summary>
        /// gets and sets the peptide evidence
        /// </summary>
        public MzIdentMlPeptideEvidence PeptideEvidence { get; set; }
    }
}
