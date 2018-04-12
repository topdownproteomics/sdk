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

            List<IProteoformModification> modifications = null;
            if (term.Tags != null && term.Tags.Count > 0)
            {
                if (modificationLookup == null)
                    throw new ProteoformGroupCreateException("Cannot lookup tag because lookup wasn't provided.");

                foreach (var tag in term.Tags)
                {
                    foreach (var descriptor in tag.Descriptors)
                    {
                        if (modificationLookup.CanHandleDescriptor(descriptor))
                        {
                            var modification = modificationLookup.GetModification(descriptor);

                            if (modification != null)
                            {
                                if (modifications == null)
                                    modifications = new List<IProteoformModification>();

                                modifications.Add(modification);
                            }
                        }
                        else
                        {
                            throw new ProteoformGroupCreateException($"Couldn't handle descriptor {descriptor.Key}:{descriptor.Value}.");
                        }
                    }
                }
            }

            return new ProteoformGroup(residues, modifications, _water);
        }

        private class ProteoformGroup : IProteoformGroup
        {
            public ProteoformGroup(IReadOnlyList<IResidue> residues,
                IReadOnlyCollection<IProteoformModification> modifications,
                IChemicalFormula water)
            {
                this.Residues = residues;
                this.Modifications = modifications;
                this.Water = water;
            }

            public IReadOnlyList<IResidue> Residues { get; }
            public IReadOnlyCollection<IProteoformModification> Modifications { get; }
            public IChemicalFormula Water { get; }

            public double GetMass(MassType massType)
            {
                return this.Water.GetMass(massType) +
                    this.Residues.Sum(x => x.GetChemicalFormula().GetMass(massType)) +
                    (this.Modifications?.Sum(x => x.GetChemicalFormula().GetMass(massType)) ?? 0.0);
            }
        }
    }
}