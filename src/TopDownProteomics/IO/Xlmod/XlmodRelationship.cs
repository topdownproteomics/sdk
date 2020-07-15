namespace TopDownProteomics.IO.Xlmod
{
    /// <summary>A relationship to another term from the XLMOD modification ontology.</summary>
    public class XlmodRelationship
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XlmodRelationship"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="id">The identifier.</param>
        public XlmodRelationship(string type, string id)
        {
            Type = type;
            Id = id;
        }

        /// <summary>The type of relationship.</summary>
        public string Type { get; }

        /// <summary>The Id of the term in the relationship.</summary>
        public string Id { get; }
    }
}