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
        /// <param name="unlocalizedTags">Unlocalized modification tags.</param>
        /// <param name="tagGroups">The tag groups.</param>
        public ProFormaTerm(string sequence, IList<ProFormaTag>? tags = null, IList<ProFormaDescriptor>? nTerminalDescriptors = null, 
            IList<ProFormaDescriptor>? cTerminalDescriptors = null, IList<ProFormaTag>? unlocalizedTags = null, 
            ICollection<ProFormaTagGroup>? tagGroups = null)
        {
            this.Sequence = sequence;
            this.NTerminalDescriptors = nTerminalDescriptors;
            this.CTerminalDescriptors = cTerminalDescriptors;
            this.Tags = tags;
            this.UnlocalizedTags = unlocalizedTags;
            this.TagGroups = tagGroups;
        }

        /// <summary>The amino acid sequence.</summary>
        public string Sequence { get; }

        /// <summary>The N-Terminal descriptor.</summary>
        public IList<ProFormaDescriptor>? NTerminalDescriptors { get; }

        /// <summary>The C-Terminal descriptor.</summary>
        public IList<ProFormaDescriptor>? CTerminalDescriptors { get; }

        /// <summary>All tags on this term.</summary>
        public IList<ProFormaTag>? Tags { get; }

        /// <summary>Descriptors for modifications that are completely unlocalized.</summary>
        public IList<ProFormaTag>? UnlocalizedTags { get; }

        /// <summary>All tag groups for this term.</summary>
        public ICollection<ProFormaTagGroup>? TagGroups { get; }
    }
}