using System.Collections.Generic;
using TopDownProteomics.Proteomics;

namespace TopDownProteomics.IO.PsiMod
{
    /// <summary>A term from the PSI-MOD modification ontology.</summary>
    public class PsiModTerm : IIdentifiable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PsiModTerm"/> class.
        /// </summary>
        public PsiModTerm(string id, string name, string definition)
        {
            this.Id = id;
            this.Name = name;
            this.Definition = definition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PsiModTerm" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="externalReferences">The external references.</param>
        /// <param name="synonyms">The synonyms.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="diffAvg">The difference average.</param>
        /// <param name="diffFormula">The difference formula.</param>
        /// <param name="diffMono">The difference mono.</param>
        /// <param name="formula">The formula.</param>
        /// <param name="massAvg">The mass average.</param>
        /// <param name="massMono">The mass mono.</param>
        /// <param name="origin">The origin amino acid.</param>
        /// <param name="source">The source.</param>
        /// <param name="terminus">The terminus.</param>
        /// <param name="isObsolete">if set to <c>true</c> [is obsolete].</param>
        /// <param name="remap">The remapping term.</param>
        /// <param name="formalCharge">The formal charge.</param>
        /// <param name="isA">The is a.</param>
        public PsiModTerm(string id, string name, string definition, ICollection<PsiModExternalReference>? externalReferences, 
            ICollection<PsiModSynonym>? synonyms, string? comment, double? diffAvg, string? diffFormula, double? diffMono, string? formula, double? massAvg, 
            double? massMono, ICollection<char>? origin, PsiModModificationSource? source, Terminus? terminus, bool isObsolete, string? remap, 
            int formalCharge, ICollection<string>? isA)
        {
            Id = id;
            ExternalReferences = externalReferences;
            Synonyms = synonyms;
            Name = name;
            Definition = definition;
            Comment = comment;
            DiffAvg = diffAvg;
            DiffFormula = diffFormula;
            DiffMono = diffMono;
            Formula = formula;
            MassAvg = massAvg;
            MassMono = massMono;
            Origin = origin;
            Source = source;
            Terminus = terminus;
            IsObsolete = isObsolete;
            Remap = remap;
            FormalCharge = formalCharge;
            IsA = isA;
        }

        /// <summary>The PSI-MOD identifier.</summary>
        public string Id { get; }

        /// <summary>The external references.</summary>
        public ICollection<PsiModExternalReference>? ExternalReferences { get; set; }

        /// <summary>The synonyms.</summary>
        public ICollection<PsiModSynonym>? Synonyms { get; set; }

        /// <summary>The name of the term.</summary>
        public string Name { get; }

        /// <summary>The term definition.</summary>
        public string Definition { get; }

        /// <summary>The term comment.</summary>
        public string? Comment { get; }

        /// <summary>The difference mass in average mass.</summary>
        public double? DiffAvg { get; }

        /// <summary>The difference formula.</summary>
        public string? DiffFormula { get; }

        /// <summary>The difference mass in monoisotopic mass.</summary>
        public double? DiffMono { get; }

        /// <summary>The whole modification and residue formula.</summary>
        public string? Formula { get; }

        /// <summary>The total average mass.</summary>
        public double? MassAvg { get; }

        /// <summary>The total monoisotopic mass.</summary>
        public double? MassMono { get; }

        /// <summary>The origin amino acid (multiple for crosslinks).</summary>
        public ICollection<char>? Origin { get; }

        /// <summary>The source of the term.</summary>
        public PsiModModificationSource? Source { get; }

        /// <summary>The terminus where this term can be applied.</summary>
        public Terminus? Terminus { get; }

        /// <summary>A value indicating whether this instance is obsolete.</summary>
        public bool IsObsolete { get; }

        /// <summary>Indicates that the term should be remapped to another term.</summary>
        public string? Remap { get; }

        /// <summary>The formal charge.</summary>
        public int FormalCharge { get; }

        /// <summary>The is-a collection.</summary>
        public ICollection<string>? IsA { get; set; }
    }
}