using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the SpectrumIdentificationProtocol element
	/// </summary>
	public class MzIdentMlSpectrumIdentificationProtocol
    {
        /// <summary>
        /// Gets and sets the id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets and sets the software id
        /// </summary>
        public string SoftwareId { get; set; }

        /// <summary>
        /// Gets and sets the search types
        /// </summary>
        public List<MzIdentMlParam> SearchTypes { get; set; }

        /// <summary>
        /// Gets and sets the search params
        /// </summary>
        public List<MzIdentMlParam> SearchParams { get; set; } = new List<MzIdentMlParam>();

        /// <summary>
        /// Gets and sets the database filter params
        /// </summary>
        public List<MzIdentMlParam> DatabaseFilterParams { get; set; } = new List<MzIdentMlParam>();

        /// <summary>
        /// Gets and sets the fragment tolerances
        /// </summary>
        public List<MzIdentMlParam> FragmentTolerances { get; set; }

        /// <summary>
        /// Gets and sets the precursor tolerances
        /// </summary>
        public List<MzIdentMlParam> PrecursorTolerances { get; set; }

        /// <summary>
        /// Gets and sets the thresholds
        /// </summary>
        public List<MzIdentMlParam> Thresholds { get; set; }
    }
}
