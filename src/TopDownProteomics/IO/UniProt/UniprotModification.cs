using System;
using System.Collections.Generic;
using TopDownProteomics.Chemistry;

namespace TopDownProteomics.IO.UniProt
{
    /// <summary>
    /// A modification from the UniProt ptmlist.txt file.
    /// </summary>
    public class UniprotModification : IIdentifiable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UniprotModification"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="accession">The accession.</param>
        /// <param name="featureKey">The feature key.</param>
        /// <param name="target">The target.</param>
        /// <param name="aminoAcidPosition">The amino acid position.</param>
        /// <param name="polypeptidePosition">The polypeptide position.</param>
        /// <param name="correctionFormula">The correction formula.</param>
        /// <param name="monoisotopicMassDifference">The monoisotopic mass difference.</param>
        /// <param name="averageMassDifference">The average mass difference.</param>
        /// <param name="cellularLocation">The cellular location.</param>
        /// <param name="taxonomicRanges">The taxonomic ranges.</param>
        /// <param name="keywords">The keywords.</param>
        /// <param name="resid">The resid.</param>
        /// <param name="psiMod">The psi mod.</param>
        /// <param name="unimod">The unimod.</param>
        public UniprotModification(string identifier, string accession, UniprotFeatureType featureKey, string target, string aminoAcidPosition, 
            string polypeptidePosition, string? correctionFormula, double monoisotopicMassDifference, double averageMassDifference, 
            string cellularLocation, ICollection<string>? taxonomicRanges, ICollection<string>? keywords, string? resid, string? psiMod, string? unimod)
        {
            Identifier = identifier;
            Accession = accession;
            FeatureKey = featureKey;
            Target = target;
            AminoAcidPosition = aminoAcidPosition;
            PolypeptidePosition = polypeptidePosition;
            CorrectionFormula = correctionFormula;
            MonoisotopicMassDifference = monoisotopicMassDifference;
            AverageMassDifference = averageMassDifference;
            CellularLocation = cellularLocation;
            TaxonomicRanges = taxonomicRanges;
            Keywords = keywords;
            Resid = resid;
            PsiMod = psiMod;
            Unimod = unimod;
        }

        /// <summary>The identifier.</summary>
        public string Id => this.Accession;

        /// <summary>The name.</summary>
        public string Name => this.Identifier;

        /// <summary>Gets or sets the identifier.</summary>
        public string Identifier { get; }

        /// <summary>Gets or sets the accession.</summary>
        public string Accession { get; }

        /// <summary>Gets or sets the feature key.</summary>
        public UniprotFeatureType FeatureKey { get; }

        /// <summary>Gets or sets the target.</summary>
        public string Target { get; }

        /// <summary>Gets or sets the amino acid position.</summary>
        public string AminoAcidPosition { get; }

        /// <summary>Gets or sets the polypeptide position.</summary>
        public string PolypeptidePosition { get; }

        /// <summary>Gets or sets the correction formula.</summary>
        public string? CorrectionFormula { get; }

        /// <summary>Gets or sets the monoisotopic mass difference.</summary>
        public double MonoisotopicMassDifference { get; }

        /// <summary>Gets or sets the average mass difference.</summary>
        public double AverageMassDifference { get; }

        /// <summary>Gets or sets the cellular location.</summary>
        public string CellularLocation { get; }

        /// <summary>Gets or sets the taxonomic ranges.</summary>
        public ICollection<string>? TaxonomicRanges { get; }

        /// <summary>Gets or sets the keywords.</summary>
        public ICollection<string>? Keywords { get; }

        /// <summary>Gets or sets the RESID identifier.</summary>
        public string? Resid { get; }

        /// <summary>Gets or sets the PSI-MOD identifier.</summary>
        public string? PsiMod { get; }

        /// <summary>The Unimod identifier mapping.</summary>
        public string? Unimod { get; }

        /// <summary>Gets the chemical formula.</summary>
        public IChemicalFormula? GetChemicalFormula(IElementProvider elementProvider)
        {
            string? formula = this.CorrectionFormula;

            if (string.IsNullOrEmpty(formula))
                return null;

            string[] cells = formula.Split(' ');

            var elements = new List<IEntityCardinality<IElement>>();

            for (int i = 0; i < cells.Length; i++)
            {
                // Find last index for element name
                int j = cells[i].Length - 1;
                while (char.IsDigit(cells[i][j]) || cells[i][j] == '-')
                {
                    j--;
                }

                string elementSymbol = cells[i].Substring(0, j + 1);
                int count = Convert.ToInt32(cells[i].Substring(j + 1));

                if (count != 0)
                    elements.Add(new EntityCardinality<IElement>(elementProvider.GetElement(elementSymbol), count));
            }

            return new ChemicalFormula(elements);
        }
    }
}