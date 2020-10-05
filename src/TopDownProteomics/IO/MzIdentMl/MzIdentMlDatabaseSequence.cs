namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the DBSequence element
	/// </summary>
	public class MzIdentMlDatabaseSequence
    {
        /// <summary>
        /// Gets and sets the Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets and sets the length
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Gets and sets the search database Id
        /// </summary>
        public string SearchDatabaseId { get; set; }

        /// <summary>
        /// Gets and sets the accession
        /// </summary>
        public string Accession { get; set; }

        /// <summary>
        /// Gets and sets the sequence
        /// </summary>
        public string Sequence { get; set; }

        /// <summary>
        /// Gets and sets the protein description
        /// </summary>
        public string ProteinDescription { get; set; }

        /// <summary>
        /// Gets and sets the taxonomy scientific name
        /// </summary>
        public string TaxonomyScientificName { get; set; }

        /// <summary>
        /// Gets and sets the taxonomy id
        /// </summary>
        public int TaxonomyId { get; set; }
    }
}
