using System;
using System.Collections.Generic;
using TopDownProteomics.Chemistry;
using TopDownProteomics.ProForma;
using TopDownProteomics.Proteomics;

namespace TopDownProteomics.IO.Resid
{
    /// <summary>
    /// Modification wrapper around a information in RESID.
    /// </summary>
    /// <seealso cref="IProFormaProteoformModification" />
    public class ResidModification : IIdentifiable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResidModification" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="formula">The formula.</param>
        /// <param name="monoisotopicMass">The monoisotopic mass.</param>
        /// <param name="averageMass">The average mass.</param>
        /// <param name="diffFormula">The difference formula.</param>
        /// <param name="diffMonoisotopicMass">The difference monoisotopic mass.</param>
        /// <param name="diffAverageMass">The difference average mass.</param>
        /// <param name="terminus">The terminus.</param>
        /// <param name="origin">The origin.</param>
        /// <param name="swissProtTerm">The swiss prot term.</param>
        /// <param name="formalCharge">The formal charge.</param>
        public ResidModification(string id, string name, string formula, double monoisotopicMass, double averageMass,
            string? diffFormula, double? diffMonoisotopicMass, double? diffAverageMass,
            Terminus? terminus, char? origin, string? swissProtTerm, int formalCharge = 0)
        {
            this.Id = id;
            this.Name = name;
            this.Formula = formula;
            this.MonoisotopicMass = monoisotopicMass;
            this.AverageMass = averageMass;
            this.DiffFormula = diffFormula;
            this.DiffMonoisotopicMass = diffMonoisotopicMass;
            this.DiffAverageMass = diffAverageMass;
            this.Terminus = terminus;
            this.Origin = origin;
            this.SwissprotTerm = swissProtTerm;
            this.FormalCharge = formalCharge;
        }

        /// <summary>Gets the identifier.</summary>
        public string Id { get; }

        /// <summary>Gets the name.</summary>
        public string Name { get; }

        /// <summary>Gets the formula.</summary>
        public string Formula { get; }

        /// <summary>Gets the monoisotopic mass.</summary>
        public double MonoisotopicMass { get; }

        /// <summary>Gets the average mass.</summary>
        public double AverageMass { get; }

        /// <summary>Gets the difference formula.</summary>
        public string? DiffFormula { get; }

        /// <summary>Gets the difference monoisotopic mass.</summary>
        public double? DiffMonoisotopicMass { get; }

        /// <summary>Gets the difference average mass.</summary>
        public double? DiffAverageMass { get; }

        /// <summary>Gets the origin amino acid.</summary>
        public char? Origin { get; }

        /// <summary>Gets the terminus.</summary>
        public Terminus? Terminus { get; }

        /// <summary>Gets the formal charge.</summary>
        public int FormalCharge { get; }

        /// <summary>The swissprot term.</summary>
        public string? SwissprotTerm { get; }

        /// <summary>Gets the chemical formula.</summary>
        public IChemicalFormula? GetChemicalFormula(IElementProvider elementProvider)
        {
            string? formula = this.DiffFormula;

            if (string.IsNullOrEmpty(formula))
                return null;

            string[] cells = formula.Split(' ');

            var elements = new List<IEntityCardinality<IElement>>();

            for (int i = 0; i < cells.Length; i += 2)
            {
                if (cells[i] == "+")
                    continue;

                int count = Convert.ToInt32(cells[i + 1]);

                if (count != 0)
                {
                    // Handle formal charge by adding or removing hydrogen atoms
                    if (this.FormalCharge != 0 && cells[i] == "H")
                        count -= this.FormalCharge;

                    elements.Add(new EntityCardinality<IElement>(elementProvider.GetElement(cells[i]), count));
                }
            }

            return new ChemicalFormula(elements);
        }
    }
}