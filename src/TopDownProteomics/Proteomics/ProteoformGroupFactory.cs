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

            List<IProteoformLocalizedModification>? localizedModifications = null;
            List<IProteoformUnlocalizedModification>? unlocalizedModifications = null;
            List<IProteoformModificationGroup>? modificationGroups = null;
            List<IProteoformGlobalModification>? globalModifications = null;
            IProteoformMassDelta? nTerminalModification = this.GetModification(term.NTerminalDescriptors, modificationLookup, "Multiple N Terminal Modifications");
            IProteoformMassDelta? cTerminalModification = this.GetModification(term.CTerminalDescriptors, modificationLookup, "Multiple C Terminal Modifications");

            if (term.Tags?.Count > 0)
            {
                foreach (var tag in term.Tags)
                {
                    IProteoformMassDelta? delta = this.GetModification(tag.Descriptors, modificationLookup,
                        $"Multiple modifications at index: {tag.ZeroBasedStartIndex}");

                    if (delta != null)
                    {
                        localizedModifications ??= new List<IProteoformLocalizedModification>();
                        localizedModifications.Add(new LocalizedModification(delta, tag.ZeroBasedStartIndex, tag.ZeroBasedEndIndex));
                    }
                }
            }

            if (term.LabileDescriptors?.Count > 0)
            {
                foreach (var item in term.LabileDescriptors)
                {
                    IProteoformMassDelta? delta = this.GetModification(item, modificationLookup);

                    if (delta != null)
                    {
                        unlocalizedModifications ??= new List<IProteoformUnlocalizedModification>();
                        unlocalizedModifications.Add(new UnlocalizedModification(delta, 1, true));
                    }
                }
            }

            if (term.UnlocalizedTags?.Count > 0)
            {
                foreach (var item in term.UnlocalizedTags)
                {
                    IProteoformMassDelta? delta = this.GetModification(item.Descriptors, modificationLookup,
                        "Multiple unlocalized descriptors on same tag.");

                    if (delta != null)
                    {
                        unlocalizedModifications ??= new List<IProteoformUnlocalizedModification>();
                        unlocalizedModifications.Add(new UnlocalizedModification(delta, item.Count, false));
                    }
                }
            }

            if (term.TagGroups?.Count > 0)
            {
                foreach (var item in term.TagGroups)
                {
                    IProteoformMassDelta? delta = this.GetModification(item, modificationLookup);

                    if (delta != null)
                    {
                        modificationGroups ??= new List<IProteoformModificationGroup>();
                        modificationGroups.Add(new TagGroupModification(delta, item.Name, 
                            item.Members.Select(x => new TagGroupModificationMember(x.ZeroBasedStartIndex, x.ZeroBasedEndIndex, x.Weight))));
                    }
                }
            }

            // Debating about doing this as a decorators around Residues. Starting with just keeping this as another mod type.
            if (term.GlobalModifications?.Count > 0)
            {
                foreach (var item in term.GlobalModifications)
                {
                    IProteoformMassDelta? delta = this.GetModification(item.Descriptors, modificationLookup,
                        "Multiple global modification descriptors on same tag.");

                    if (delta != null)
                    {
                        globalModifications ??= new List<IProteoformGlobalModification>();
                        globalModifications.Add(new GlobalModification(delta, item.TargetAminoAcids));
                    }
                }
            }

            return new ProteoformGroup(residues, nTerminalModification, cTerminalModification, localizedModifications, unlocalizedModifications,
                modificationGroups, globalModifications, _water);
        }

        /// <summary>
        /// Creates the proteoform group for a simple sequence of amino acids.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        /// <returns></returns>
        public IProteoformGroup CreateProteoformGroup(string sequence)
        {
            var residues = sequence.Select(x => _residueProvider.GetResidue(x)).ToArray();

            return new ProteoformGroup(residues, null, null, null, null, null, null, _water);
        }

        private class LocalizedModification : ProteoformModificationBase, IProteoformLocalizedModification
        {
            public LocalizedModification(IProteoformMassDelta modificationDelta, int zeroBasedStartIndex, int zeroBasedEndIndex)
                : base(modificationDelta)
            {
                this.ZeroBasedStartIndex = zeroBasedStartIndex;
                this.ZeroBasedEndIndex = zeroBasedEndIndex;
            }

            public int ZeroBasedStartIndex { get; }

            public int ZeroBasedEndIndex { get; }
        }

        private class UnlocalizedModification : ProteoformModificationBase, IProteoformUnlocalizedModification
        {
            public UnlocalizedModification(IProteoformMassDelta modificationDelta, int count, bool isLabile)
                : base(modificationDelta)
            {
                this.Count = count;
                this.IsLabile = isLabile;
            }

            public int Count { get; }

            public bool IsLabile { get; }
        }

        private class TagGroupModification : ProteoformModificationBase, IProteoformModificationGroup
        {
            public TagGroupModification(IProteoformMassDelta modificationDelta, string groupName, IReadOnlyCollection<IProteoformModificationGroupMember> members)
                : base(modificationDelta)
            {
                this.GroupName = groupName;
                this.Members = members;
            }

            public string GroupName { get; }

            public IReadOnlyCollection<IProteoformModificationGroupMember> Members { get; }
        }

        private class TagGroupModificationMember : IProteoformModificationGroupMember
        {
            public TagGroupModificationMember(int zeroBasedStartIndex, int zeroBasedEndIndex, double weight)
            {
                Weight = weight;
                ZeroBasedStartIndex = zeroBasedStartIndex;
                ZeroBasedEndIndex = zeroBasedEndIndex;
            }

            public double Weight { get; }

            public int ZeroBasedStartIndex { get; }

            public int ZeroBasedEndIndex { get; }
        }

        private class GlobalModification : ProteoformModificationBase, IProteoformGlobalModification
        {
            public GlobalModification(IProteoformMassDelta modificationDelta, ICollection<char>? targetAminoAcids) 
                : base(modificationDelta)
            {
                this.TargetAminoAcids = targetAminoAcids;
            }

            public ICollection<char>? TargetAminoAcids { get; }
        }

        private IProteoformMassDelta? GetModification(IList<ProFormaDescriptor>? descriptors, IProteoformModificationLookup modificationLookup,
            string multipleModsErrorMessage)
        {
            IProteoformMassDelta? modification = null;

            if (descriptors != null && descriptors.Count > 0)
            {
                if (modificationLookup == null)
                    throw new ProteoformGroupCreateException("Cannot lookup tag because lookup wasn't provided.");

                foreach (var descriptor in descriptors)
                {
                    IProteoformMassDelta? mod = null;

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

        private IProteoformMassDelta? GetModification(IProFormaDescriptor? descriptor, IProteoformModificationLookup modificationLookup)
        {
            IProteoformMassDelta? modification = null;

            if (descriptor != null)
            {
                if (modificationLookup == null)
                    throw new ProteoformGroupCreateException("Cannot lookup tag because lookup wasn't provided.");

                if (modificationLookup.CanHandleDescriptor(descriptor))
                    return modificationLookup.GetModification(descriptor);

                throw new ProteoformGroupCreateException($"Couldn't handle descriptor {descriptor}.");
            }

            return modification;
        }

        private class ProteoformGroup : IProteoformGroup
        {
            public ProteoformGroup(IReadOnlyList<IResidue> residues, IProteoformMassDelta? nTerminalModification, 
                IProteoformMassDelta? cTerminalModification, 
                IReadOnlyCollection<IProteoformLocalizedModification>? localizedModifications, 
                IReadOnlyCollection<IProteoformUnlocalizedModification>? unlocalizedModifications, 
                IReadOnlyCollection<IProteoformModificationGroup>? modificationGroups, 
                IReadOnlyCollection<IProteoformGlobalModification>? globalModifications, 
                IChemicalFormula water)
            {
                Residues = residues;
                NTerminalModification = nTerminalModification;
                CTerminalModification = cTerminalModification;
                LocalizedModifications = localizedModifications;
                UnlocalizedModifications = unlocalizedModifications;
                ModificationGroups = modificationGroups;
                GlobalModifications = globalModifications;
                Water = water;
            }

            public IReadOnlyList<IResidue> Residues { get; }
            public IProteoformMassDelta? NTerminalModification { get; }
            public IProteoformMassDelta? CTerminalModification { get; }
            public IChemicalFormula Water { get; }

            public IReadOnlyCollection<IProteoformLocalizedModification>? LocalizedModifications { get; }
            public IReadOnlyCollection<IProteoformUnlocalizedModification>? UnlocalizedModifications { get; }
            public IReadOnlyCollection<IProteoformModificationGroup>? ModificationGroups { get; }
            public IReadOnlyCollection<IProteoformGlobalModification>? GlobalModifications { get; }

            public double GetMass(MassType massType)
            {
                return this.Water.GetMass(massType) +
                    this.Residues.Sum(x => x.GetChemicalFormula().GetMass(massType)) +
                    (this.LocalizedModifications?.Sum(x => x.ModificationDelta.GetMass(massType)) ?? 0.0) +
                    (this.UnlocalizedModifications?.Sum(x => x.ModificationDelta.GetMass(massType)) ?? 0.0) +
                    (this.ModificationGroups?.Sum(x => x.ModificationDelta.GetMass(massType)) ?? 0.0) +
                    (this.GlobalModifications?.Sum(x => x.ModificationDelta.GetMass(massType)) ?? 0.0) +
                    (this.NTerminalModification?.GetMass(massType) ?? 0.0) +
                    (this.CTerminalModification?.GetMass(massType) ?? 0.0);
            }
        }
    }
}