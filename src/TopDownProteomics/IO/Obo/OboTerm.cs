using System.Collections.Generic;

namespace TopDownProteomics.IO.Obo
{
    /// <summary>
    /// Open Biological and Biomedical Ontologies Term.
    /// </summary>
    public class OboTerm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OboTerm"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="valuePairs">The value pairs.</param>
        public OboTerm(string id, string name, ICollection<OboTagValuePair>? valuePairs)
        {
            this.Id = id;
            this.Name = name;
            this.ValuePairs = valuePairs;
        }

        /// <summary>The identifier.</summary>
        public string Id { get; }

        /// <summary>The name.</summary>
        public string Name { get; }

        /// <summary>The value pairs.</summary>
        public ICollection<OboTagValuePair>? ValuePairs { get; }
    }
}