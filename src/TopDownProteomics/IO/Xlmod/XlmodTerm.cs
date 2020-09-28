using System.Collections.Generic;

namespace TopDownProteomics.IO.Xlmod
{
    /// <summary>A term from the XLMOD modification ontology.</summary>
    public class XlmodTerm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XlmodTerm"/> class.
        /// </summary>
        public XlmodTerm(string id, string name, string definition)
        {
            this.Id = id;
            this.Name = name;
            this.Definition = definition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XlmodTerm"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="externalReferences">The external references.</param>
        /// <param name="isA">The is a.</param>
        /// <param name="relationships">The relationships.</param>
        /// <param name="synonyms">The synonyms.</param>
        /// <param name="propertyValues">The property values.</param>
        public XlmodTerm(string id, string name, string definition, ICollection<XlmodExternalReference>? externalReferences,
            ICollection<string>? isA, ICollection<XlmodRelationship>? relationships, ICollection<XlmodSynonym>? synonyms, ICollection<XlmodProperty>? propertyValues) 
            : this(id, name, definition)
        {
            ExternalReferences = externalReferences;
            IsA = isA;
            Relationships = relationships;
            Synonyms = synonyms;
            PropertyValues = propertyValues;
        }

        /// <summary>The XLMOD identifier.</summary>
        public string Id { get; }

        /// <summary>The name of the term.</summary>
        public string Name { get; }

        /// <summary>The term definition.</summary>
        public string Definition { get; }

        /// <summary>The external references.</summary>
        public ICollection<XlmodExternalReference>? ExternalReferences { get; set; }

        /// <summary>The is-a collection.</summary>
        public ICollection<string>? IsA { get; set; }

        /// <summary>The synonyms.</summary>
        public ICollection<XlmodRelationship>? Relationships { get; set; }

        /// <summary>The synonyms.</summary>
        public ICollection<XlmodSynonym>? Synonyms { get; set; }

        /// <summary>The synonyms.</summary>
        public ICollection<XlmodProperty>? PropertyValues { get; set; }
    }
}