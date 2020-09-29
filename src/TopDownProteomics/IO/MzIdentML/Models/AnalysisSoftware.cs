using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.IO.MzIdentML.Models
{
    /// <summary>
    /// Corresponds to AnalysisSoftware element
    /// </summary>
    public class AnalysisSoftware
    {
        /// <summary>
        /// Gets and sets the Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets and sets the Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets and sets the Version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets and sets the URI
        /// </summary>
        public string Uri { get; set; }
    }
}
