using System;
using System.Collections.Generic;

namespace TopDownProteomics.ProForma
{
    /// <summary>
    /// Represents a ProForma string in memory.
    /// </summary>
    public class ProFormaTerm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProFormaTerm" /> class.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="nTerminalDescriptors">The n terminal descriptors.</param>
        /// <param name="cTerminalDescriptors">The c terminal descriptors.</param>
        /// <param name="labileDescriptors">The labile modification descriptors.</param>
        /// <param name="unlocalizedTags">Unlocalized modification tags.</param>
        /// <param name="tagGroups">The tag groups.</param>
        /// <param name="globalModifications">The global modifications.</param>
        /// <param name="nTerminalModifications">The N-terminal modifications, one descriptor list per modification. Pass
        /// either this or <paramref name="nTerminalDescriptors"/>, not both.</param>
        /// <param name="cTerminalModifications">The C-terminal modifications, one descriptor list per modification. Pass
        /// either this or <paramref name="cTerminalDescriptors"/>, not both.</param>
        public ProFormaTerm(string sequence, IList<ProFormaTag>? tags = null, IList<ProFormaDescriptor>? nTerminalDescriptors = null, 
            IList<ProFormaDescriptor>? cTerminalDescriptors = null, IList<ProFormaDescriptor>? labileDescriptors = null,
            IList<ProFormaUnlocalizedTag>? unlocalizedTags = null, ICollection<ProFormaTagGroup>? tagGroups = null,
            IList<ProFormaGlobalModification>? globalModifications = null,
            IList<IList<ProFormaDescriptor>>? nTerminalModifications = null, IList<IList<ProFormaDescriptor>>? cTerminalModifications = null)
        {
            if (nTerminalDescriptors != null && nTerminalModifications != null)
                throw new ArgumentException("Pass the N-terminal modifications either as nTerminalDescriptors (one) or as " +
                    "nTerminalModifications (any number), not both.", nameof(nTerminalModifications));

            if (cTerminalDescriptors != null && cTerminalModifications != null)
                throw new ArgumentException("Pass the C-terminal modifications either as cTerminalDescriptors (one) or as " +
                    "cTerminalModifications (any number), not both.", nameof(cTerminalModifications));

            this.Sequence = sequence;
            this.NTerminalModifications = nTerminalModifications ?? AsModifications(nTerminalDescriptors);
            this.CTerminalModifications = cTerminalModifications ?? AsModifications(cTerminalDescriptors);
            this.NTerminalDescriptors = nTerminalDescriptors ?? First(nTerminalModifications);
            this.CTerminalDescriptors = cTerminalDescriptors ?? First(cTerminalModifications);
            this.LabileDescriptors = labileDescriptors;
            this.Tags = tags;
            this.UnlocalizedTags = unlocalizedTags;
            this.TagGroups = tagGroups;
            this.GlobalModifications = globalModifications;
        }

        /// <summary>The amino acid sequence.</summary>
        public string Sequence { get; }

        /// <summary>Modifications that apply globally based on a target or targets.</summary>
        public IList<ProFormaGlobalModification>? GlobalModifications { get; }

        /// <summary>
        /// N-Terminal descriptors: the first N-terminal modification. A term can carry several (ProForma 2.1, section 6.3);
        /// <see cref="NTerminalModifications"/> holds them all.
        /// </summary>
        public IList<ProFormaDescriptor>? NTerminalDescriptors { get; }

        /// <summary>
        /// C-Terminal descriptors: the first C-terminal modification. A term can carry several (ProForma 2.1, section 6.3);
        /// <see cref="CTerminalModifications"/> holds them all.
        /// </summary>
        public IList<ProFormaDescriptor>? CTerminalDescriptors { get; }

        /// <summary>
        /// N-terminal modifications in the order written, one descriptor list per modification: [Acetyl][Carbamyl]-QPEPTIDE
        /// has two (ProForma 2.1, section 6.3). One descriptor list is one modification, described one or more ways
        /// ([Acetyl|UNIMOD:1]).
        /// </summary>
        public IList<IList<ProFormaDescriptor>>? NTerminalModifications { get; }

        /// <summary>
        /// C-terminal modifications in the order written, one descriptor list per modification: PEPTIDEG-[Methyl][Amidated]
        /// has two (ProForma 2.1, section 6.3).
        /// </summary>
        public IList<IList<ProFormaDescriptor>>? CTerminalModifications { get; }

        /// <summary>Labile modifications (not visible in the fragmentation MS2 spectrum) descriptors.</summary>
        public IList<ProFormaDescriptor>? LabileDescriptors { get; }

        /// <summary>All tags on this term.</summary>
        public IList<ProFormaTag>? Tags { get; }

        /// <summary>Descriptors for modifications that are completely unlocalized.</summary>
        public IList<ProFormaUnlocalizedTag>? UnlocalizedTags { get; }

        /// <summary>All tag groups for this term.</summary>
        public ICollection<ProFormaTagGroup>? TagGroups { get; }

        private static IList<IList<ProFormaDescriptor>>? AsModifications(IList<ProFormaDescriptor>? descriptors) =>
            descriptors == null ? null : new List<IList<ProFormaDescriptor>> { descriptors };

        private static IList<ProFormaDescriptor>? First(IList<IList<ProFormaDescriptor>>? modifications) =>
            modifications != null && modifications.Count > 0 ? modifications[0] : null;
    }
}