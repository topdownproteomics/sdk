namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to cvParam element
	/// </summary>
	public class MzIdentMlParam
    {
        /// <summary>
        /// Gets and sets the accession
        /// </summary>
        public string Accession { get; set; }

        /// <summary>
        /// Gets and sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets and sets the value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets and sets the unit accession
        /// </summary>
        public string UnitAccession { get; set; }

        /// <summary>
        /// Gets and sets the unit name
        /// </summary>
        public string UnitName { get; set; }
    }
}
