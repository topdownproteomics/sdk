using System;
using System.Collections.Generic;

namespace TopDownProteomics.IO.UniProt
{
    /// <summary>
    /// A modification from the UniProt ptmlist.txt file.
    /// </summary>
    public class UniprotModification : IIdentifiable
    {
        /// <summary>
        /// The identifier.
        /// </summary>
        public int Id => Convert.ToInt32(this.Accession.Substring(4));

        /// <summary>
        /// The name.
        /// </summary>
        public string Name => this.Identifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="UniprotModification" /> class.
        /// </summary>
        public UniprotModification() { }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the accession.
        /// </summary>
        public string Accession { get; set; }

        /// <summary>
        /// Gets or sets the feature key.
        /// </summary>
        public UniprotFeatureType FeatureKey { get; set; }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// Gets or sets the amino acid position.
        /// </summary>
        public string AminoAcidPosition { get; set; }

        /// <summary>
        /// Gets or sets the polypeptide position.
        /// </summary>
        public string PolypeptidePosition { get; set; }

        /// <summary>
        /// Gets or sets the correction formula.
        /// </summary>
        public string CorrectionFormula { get; set; }

        /// <summary>
        /// Gets or sets the monoisotopic mass difference.
        /// </summary>
        public double MonoisotopicMassDifference { get; set; }

        /// <summary>
        /// Gets or sets the average mass difference.
        /// </summary>
        public double AverageMassDifference { get; set; }

        /// <summary>
        /// Gets or sets the cellular location.
        /// </summary>
        public string CellularLocation { get; set; }

        /// <summary>
        /// Gets or sets the taxonomic ranges.
        /// </summary>
        public List<string> TaxonomicRanges { get; set; }

        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        public List<string> Keywords { get; set; }

        /// <summary>
        /// Gets or sets the RESID identifier.
        /// </summary>
        public string Resid { get; set; }

        /// <summary>
        /// Gets or sets the PSI-MOD identifier.
        /// </summary>
        public string PsiMod { get; set; }
    }
}