using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.IO.MzIdentML.Models
{
    /// <summary>
    /// Corresponds to the SpectrumIdentificationProtocol element
    /// </summary>
    public class SpectrumIdentificationProtocol
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
        public List<CvParam> SearchTypes { get; set; }

        /// <summary>
        /// Gets and sets the search params
        /// </summary>
        public List<CvParam> SearchParams { get; set; } = new List<CvParam>();

        /// <summary>
        /// Gets and sets the database filter params
        /// </summary>
        public List<CvParam> DatabaseFilterParams { get; set; } = new List<CvParam>();

        /// <summary>
        /// Gets and sets the fragment tolerances
        /// </summary>
        public List<CvParam> FragmentTolerances { get; set; }

        /// <summary>
        /// Gets and sets the precursor tolerances
        /// </summary>
        public List<CvParam> PrecursorTolerances { get; set; }

        /// <summary>
        /// Gets and sets the  thresholds
        /// </summary>
        public List<CvParam> Thresholds { get; set; }
    }
}
