using System;
using System.Collections.Generic;
using System.Linq;
using TopDownProteomics.Biochemistry;
using TopDownProteomics.Chemistry;
using TopDownProteomics.ProForma;
using TopDownProteomics.ProForma.Validation;

namespace TopDownProteomics.Proteomics
{
    /// <summary>
    /// Creates IProteoformGroup objects from a ProForma Term.
    /// </summary>
    public class ProteoformGroupFactory
    {
        IElementProvider _elementProvider;
        IResidueProvider _residueProvider;
        IChemicalFormula _water;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProteoformGroupFactory"/> class.
        /// </summary>
        /// <param name="elementProvider">The element provider.</param>
        /// <param name="residueProvider">The residue provider.</param>
        public ProteoformGroupFactory(IElementProvider elementProvider, IResidueProvider residueProvider)
        {
            _elementProvider = elementProvider;
            _residueProvider = residueProvider;
            _water = new ChemicalFormula(new[]
            {
                new EntityCardinality<IElement>(_elementProvider.GetElement(1), 2),
                new EntityCardinality<IElement>(_elementProvider.GetElement(8), 1),
            });
        }

        /// <summary>
        /// Creates the proteoform group.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="modificationLookup">The modification lookup.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">term</exception>
        /// <exception cref="ProteoformGroupCreateException">
        /// Cannot lookup tag because lookup wasn't provided.
        /// or
        /// </exception>
        public IProteoformGroup CreateProteoformGroup(ProFormaTerm term, IProteoformModificationLookup modificationLookup)
        {
            if (term == null) throw new ArgumentNullException(nameof(term));

            var residues = term.Sequence.Select(x => _residueProvider.GetResidue(x)).ToArray();

            List<IProteoformModificationWithIndex>? modifications = null;
            IProteoformModification? nTerminalModification = this.GetModification(term.NTerminalDescriptors, modificationLookup, "Multiple N Terminal Modifications");
            IProteoformModification? cTerminalModification = this.GetModification(term.CTerminalDescriptors, modificationLookup, "Multiple C Terminal Modifications");

            if (term.Tags?.Count > 0)
            {
                foreach (var tag in term.Tags)
                {
                    IProteoformModification? modificationAtIndex = this.GetModification(tag.Descriptors, modificationLookup, 
                        $"Multiple modifications at index: {tag.ZeroBasedStartIndex}");

                    // Lazy create the modifications list and add
                    if (modificationAtIndex != null)
                    {
                        if (modifications == null)
                            modifications = new List<IProteoformModificationWithIndex>();

                        IProteoformModificationWithIndex proteoformModificationWithIndex = new ProteoformModificationWithIndex(modificationAtIndex, tag.ZeroBasedStartIndex);
                        modifications.Add(proteoformModificationWithIndex);
                    }
                }
            }

            return new ProteoformGroup(residues, nTerminalModification, cTerminalModification, modifications, _water);
        }

        private IProteoformModification? GetModification(IList<ProFormaDescriptor>? descriptors, IProteoformModificationLookup modificationLookup,
            string multipleModsErrorMessage)
        {
            IProteoformModification? modification = null;

            if (descriptors != null && descriptors.Count > 0)
            {
                if (modificationLookup == null)
                    throw new ProteoformGroupCreateException("Cannot lookup tag because lookup wasn't provided.");

                foreach (var descriptor in descriptors)
                {
                    IProteoformModification? mod = null;

                    if (modificationLookup.CanHandleDescriptor(descriptor))
                    {
                        mod = modificationLookup.GetModification(descriptor);
                    }
                    else
                    {
                        throw new ProteoformGroupCreateException($"Couldn't handle descriptor {descriptor}.");
                    }

                    if (modification == null)
                    {
                        modification = mod;
                    }
                    else if (mod != null)
                    {
                        if (mod is IHasChemicalFormula form1 &&
                            modification is IHasChemicalFormula form2 &&
                            !form1.GetChemicalFormula().Equals(form2.GetChemicalFormula()))
                        {
                            throw new ProteoformGroupCreateException(multipleModsErrorMessage);
                        }
                    }
                }
            }

            return modification;
        }

        private class ProteoformGroup : IProteoformGroup
        {
            public ProteoformGroup(IReadOnlyList<IResidue> residues,
                IProteoformModification? nTerminalModification,
                IProteoformModification? cTerminalModification,
                IReadOnlyCollection<IProteoformModificationWithIndex>? modifications,
                IChemicalFormula water)
            {
                this.Residues = residues;
                this.NTerminalModification = nTerminalModification;
                this.CTerminalModification = cTerminalModification;
                this.Modifications = modifications;
                this.Water = water;
            }

            public IReadOnlyList<IResidue> Residues { get; }
            public IProteoformModification? NTerminalModification { get; }
            public IProteoformModification? CTerminalModification { get; }
            public IReadOnlyCollection<IProteoformModificationWithIndex>? Modifications { get; }
            public IChemicalFormula Water { get; }

            public double GetMass(MassType massType)
            {
                return this.Water.GetMass(massType) +
                    this.Residues.Sum(x => x.GetChemicalFormula().GetMass(massType)) +
                    (this.Modifications?.Sum(x => x.Modification.GetMass(massType)) ?? 0.0) +
                    (this.NTerminalModification?.GetMass(massType) ?? 0.0) +
                    (this.CTerminalModification?.GetMass(massType) ?? 0.0);
            }
        }
    }
}