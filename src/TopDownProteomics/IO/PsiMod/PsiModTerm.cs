using System.Collections.Generic;
using TopDownProteomics.Proteomics;

namespace TopDownProteomics.IO.PsiMod
{
    /// <summary>
    /// A term from the PSI-MOD modification ontology.
    /// </summary>
    public class PsiModTerm : IIdentifiable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PsiModTerm"/> class.
        /// </summary>
        public PsiModTerm()
        {
            this.IsA = new List<int>();
            this.ExternalReferences = new List<PsiModExternalReference>();
            this.Synonyms = new List<PsiModSynonym>();
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the external references.
        /// </summary>
        public ICollection<PsiModExternalReference> ExternalReferences { get; set; }

        /// <summary>
        /// Gets or sets the synonyms.
        /// </summary>
        public ICollection<PsiModSynonym> Synonyms { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the definition.
        /// </summary>
        public string Definition { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the difference average.
        /// </summary>
        public double? DiffAvg { get; set; }

        /// <summary>
        /// Gets or sets the difference formula.
        /// </summary>
        public string DiffFormula { get; set; }

        /// <summary>
        /// Gets or sets the difference monoisotopic.
        /// </summary>
        public double? DiffMono { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        public string Formula { get; set; }

        /// <summary>
        /// Gets or sets the mass average.
        /// </summary>
        public double? MassAvg { get; set; }

        /// <summary>
        /// Gets or sets the mass monoisotopic.
        /// </summary>
        public double? MassMono { get; set; }

        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        public char? Origin { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public PsiModModificationSource? Source { get; set; }

        /// <summary>
        /// Gets or sets the terminus.
        /// </summary>
        public Terminus? Terminus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is obsolete.
        /// </summary>
        public bool IsObsolete { get; set; }

        /// <summary>
        /// Gets or sets the formal charge.
        /// </summary>
        public int FormalCharge { get; set; }

        /// <summary>
        /// Gets or sets the is-a collection.
        /// </summary>
        public ICollection<int> IsA { get; set; }
    }
}
