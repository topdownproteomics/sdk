using System.Collections.Generic;
using TopDownProteomics.Biochemistry;
using TopDownProteomics.Chemistry;
using TopDownProteomics.Chemistry.Unimod;

namespace TopDownProteomics.IO.Unimod
{
    /// <summary>
    /// A modification from the UniMod database.
    /// </summary>
    public class UnimodModification : IIdentifiable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnimodModification" /> class.
        /// </summary>
        /// <param name="id">The modification identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="definitionRemainder">The definition remainder.</param>
        /// <param name="deltaComposition">The delta composition.</param>
        /// <param name="deltaMonoisotopicMass">The delta monoisotopic mass.</param>
        /// <param name="deltaAverageMass">The delta average mass.</param>
        /// <param name="allowedResidueSymbols"></param>
        /// <param name="allowedTermini"></param>
        /// <param name="classifications"></param>
        public UnimodModification(string id, string name, string definition, string definitionRemainder, string deltaComposition, double deltaMonoisotopicMass,
            double deltaAverageMass, ICollection<char>? allowedResidueSymbols, ModificationTerminalSpecificity allowedTermini, ICollection<string> classifications)
        {
            this.Id = id;
            this.Name = name;
            this.Definition = definition;
            this.DefinitionRemainder = definitionRemainder;
            this.DeltaComposition = deltaComposition;
            this.DeltaMonoisotopicMass = deltaMonoisotopicMass;
            this.DeltaAverageMass = deltaAverageMass;
            this.AllowedResidueSymbols = allowedResidueSymbols;
            this.AllowedTermini = allowedTermini;
            this.Classifications = classifications;
        }

        /// <summary>Gets the modification identifier.</summary>
        public string Id { get; }

        /// <summary>Gets the name.</summary>
        public string Name { get; }

        /// <summary>Gets the definition.</summary>
        public string Definition { get; }

        /// <summary>Gets the definition remainder (area in the square brackets).</summary>
        public string DefinitionRemainder { get; }

        /// <summary>Gets the delta composition.</summary>
        public string DeltaComposition { get; }

        /// <summary>Gets the delta monoisotopic mass.</summary>
        public double DeltaMonoisotopicMass { get; }

        /// <summary>Gets the delta average mass.</summary>
        public double DeltaAverageMass { get; }

        /// <summary>The symbols of the residues allowed by this modification.</summary>
        public ICollection<char>? AllowedResidueSymbols { get; }

        /// <summary>The terminus that is allowed by this modification.</summary>
        public ModificationTerminalSpecificity AllowedTermini { get; }

        /// <summary>The classifications on this modification.</summary>
        public ICollection<string> Classifications { get; }

        /// <summary>Gets the chemical formula.</summary>
        public ChemicalFormula GetChemicalFormula(IUnimodCompositionAtomProvider atomProvider)
        {
            var composition = UnimodComposition.CreateFromFormula(this.DeltaComposition, atomProvider);

            return composition.GetChemicalFormula();
        }
    }
}