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
        /// <remarks>
        /// Each descriptor list is one modification. Use <see cref="CreateFromModifications"/> for a terminus or a labile
        /// group carrying several.
        /// </remarks>
        public ProFormaTerm(string sequence, IList<ProFormaTag>? tags = null, IList<ProFormaDescriptor>? nTerminalDescriptors = null, 
            IList<ProFormaDescriptor>? cTerminalDescriptors = null, IList<ProFormaDescriptor>? labileDescriptors = null,
            IList<ProFormaUnlocalizedTag>? unlocalizedTags = null, ICollection<ProFormaTagGroup>? tagGroups = null,
            IList<ProFormaGlobalModification>? globalModifications = null)
            : this(sequence, tags, AsModifications(nTerminalDescriptors), AsModifications(cTerminalDescriptors),
                  AsModifications(labileDescriptors), unlocalizedTags, tagGroups, globalModifications)
        {
        }

        private ProFormaTerm(string sequence, IList<ProFormaTag>? tags, IList<IList<ProFormaDescriptor>>? nTerminalModifications,
            IList<IList<ProFormaDescriptor>>? cTerminalModifications, IList<IList<ProFormaDescriptor>>? labileModifications,
            IList<ProFormaUnlocalizedTag>? unlocalizedTags, ICollection<ProFormaTagGroup>? tagGroups,
            IList<ProFormaGlobalModification>? globalModifications)
        {
            this.Sequence = sequence;
            this.NTerminalModifications = nTerminalModifications;
            this.CTerminalModifications = cTerminalModifications;
            this.LabileModifications = labileModifications;
            this.Tags = tags;
            this.UnlocalizedTags = unlocalizedTags;
            this.TagGroups = tagGroups;
            this.GlobalModifications = globalModifications;
        }

        /// <summary>
        /// Creates a term whose termini and labile groups may each carry several modifications, one descriptor list per
        /// modification: [Acetyl][Carbamyl]-QPEPTIDE (ProForma 2.1, section 6.3), {Glycan:Hex}{Glycan:NeuAc}EMEVNESPEK
        /// (ProForma 2.0, section 4.3.2).
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="nTerminalModifications">The N-terminal modifications, one descriptor list per modification.</param>
        /// <param name="cTerminalModifications">The C-terminal modifications, one descriptor list per modification.</param>
        /// <param name="labileModifications">The labile modifications, one descriptor list per modification.</param>
        /// <param name="unlocalizedTags">Unlocalized modification tags.</param>
        /// <param name="tagGroups">The tag groups.</param>
        /// <param name="globalModifications">The global modifications.</param>
        public static ProFormaTerm CreateFromModifications(string sequence, IList<ProFormaTag>? tags = null,
            IList<IList<ProFormaDescriptor>>? nTerminalModifications = null, IList<IList<ProFormaDescriptor>>? cTerminalModifications = null,
            IList<IList<ProFormaDescriptor>>? labileModifications = null, IList<ProFormaUnlocalizedTag>? unlocalizedTags = null,
            ICollection<ProFormaTagGroup>? tagGroups = null, IList<ProFormaGlobalModification>? globalModifications = null)
        {
            return new ProFormaTerm(sequence, tags, nTerminalModifications, cTerminalModifications, labileModifications,
                unlocalizedTags, tagGroups, globalModifications);
        }

        /// <summary>The amino acid sequence.</summary>
        public string Sequence { get; }

        /// <summary>Modifications that apply globally based on a target or targets.</summary>
        public IList<ProFormaGlobalModification>? GlobalModifications { get; }

        /// <summary>
        /// N-Terminal descriptors: the first N-terminal modification. A term can carry several (ProForma 2.1, section 6.3);
        /// <see cref="NTerminalModifications"/> holds them all. This is the first list in it, not a copy.
        /// </summary>
        public IList<ProFormaDescriptor>? NTerminalDescriptors => First(this.NTerminalModifications);

        /// <summary>
        /// C-Terminal descriptors: the first C-terminal modification. A term can carry several (ProForma 2.1, section 6.3);
        /// <see cref="CTerminalModifications"/> holds them all. This is the first list in it, not a copy.
        /// </summary>
        public IList<ProFormaDescriptor>? CTerminalDescriptors => First(this.CTerminalModifications);

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

        /// <summary>
        /// Labile modifications (not visible in the fragmentation MS2 spectrum) descriptors: the first labile modification.
        /// A term can carry several (ProForma 2.0, section 4.3.2); <see cref="LabileModifications"/> holds them all. This is
        /// the first list in it, not a copy.
        /// </summary>
        public IList<ProFormaDescriptor>? LabileDescriptors => First(this.LabileModifications);

        /// <summary>
        /// Labile modifications in the order written, one descriptor list per modification:
        /// {Glycan:Hex}{Glycan:NeuAc}EMEVNESPEK has two (ProForma 2.0, section 4.3.2).
        /// </summary>
        public IList<IList<ProFormaDescriptor>>? LabileModifications { get; }

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