using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.IO.MzIdentML.Models
{
    /// <summary>
    /// Corresponds to cvParam element
    /// </summary>
    public class CvParam: ICvParam
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
    }
}
