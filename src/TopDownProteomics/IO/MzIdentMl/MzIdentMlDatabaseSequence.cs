using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
    /// <summary>
    /// Corresponds to the DBSequence element
    /// </summary>
    public class MzIdentMlDatabaseSequence
    {
        /// <summary>
        /// Instantiates with the required parameters
        /// </summary>
        /// <param name="id"></param>
        /// <param name="accession"></param>
        /// <param name="searchDatabaseId"></param>
		public MzIdentMlDatabaseSequence(string id, string accession, string searchDatabaseId)
        {
            this.Id = id;
            this.Accession = accession;
            this.SearchDatabaseId = searchDatabaseId;
        }

        /// <summary>
        /// Gets and sets the Id
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets and sets the search database Id
        /// </summary>
        public string SearchDatabaseId { get; }

        /// <summary>
        /// Gets and sets the accession
        /// </summary>
        public string Accession { get; }

        /// <summary>
        /// Gets and sets the length
        /// </summary>
        public int? Length { get; set; }

        /// <summary>
        /// Gets and sets the sequence
        /// </summary>
        public string? Sequence { get; set; }

        /// <summary>
        /// Gets and sets the cvParams
        /// </summary>
		public List<MzIdentMlCvParam>? CvParams { get; set; }

        /// <summary>
        /// Gets and sets the userParams
        /// </summary>
		public List<MzIdentMlUserParam>? UserParams { get; set; }

        /// <summary>
        /// Gets and sets the protein description
        /// </summary>
        public string? ProteinDescription { get; set; }

        /// <summary>
        /// Gets and sets the taxonomy scientific name
        /// </summary>
        public string? TaxonomyScientificName { get; set; }

        /// <summary>
        /// Gets and sets the taxon ID
        /// </summary>
        public int? TaxonId { get; set; }
	}
}
