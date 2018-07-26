using System.Collections.Generic;

namespace TopDownProteomics.IO.Obo
{
    /// <summary>
    /// Open Biological and Biomedical Ontologies Term.
    /// </summary>
    public class OboTerm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OboTerm" /> class.
        /// </summary>
        public OboTerm()
        {
            this.ValuePairs = new List<OboTagValuePair>();
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value pairs.
        /// </summary>
        public ICollection<OboTagValuePair> ValuePairs { get; set; }
    }
}