using System.Collections.Generic;

namespace TopDownProteomics.ProForma
{
    /// <summary>
    /// Represents a ProForma string in memory.
    /// </summary>
    public class ProFormaTerm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProFormaTerm"/> class.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        /// <param name="unlocalizedTags">Unlocalized modification tags.</param>
        /// <param name="nTerminalDescriptors">The n terminal descriptors.</param>
        /// <param name="cTerminalDescriptors">The c terminal descriptors.</param>
        /// <param name="tags">The tags.</param>
        public ProFormaTerm(string sequence, IList<ProFormaTag> unlocalizedTags, IList<ProFormaDescriptor> nTerminalDescriptors, 
            IList<ProFormaDescriptor> cTerminalDescriptors, IList<ProFormaTag> tags)
        {
            this.Sequence = sequence;
            this.UnlocalizedTags = unlocalizedTags;
            this.NTerminalDescriptors = nTerminalDescriptors;
            this.CTerminalDescriptors = cTerminalDescriptors;
            this.Tags = tags;
        }

        /// <summary>
        /// Gets the amino acid sequence.
        /// </summary>
        public string Sequence { get; }

        /// <summary>
        /// Unlocalized modification descriptors.
        /// </summary>
        public IList<ProFormaTag> UnlocalizedTags { get; }

        /// <summary>
        /// The optional N-Terminal descriptor.
        /// </summary>
        public IList<ProFormaDescriptor> NTerminalDescriptors { get; }

        /// <summary>
        /// The optional C-Terminal descriptor.
        /// </summary>
        public IList<ProFormaDescriptor> CTerminalDescriptors { get; }

        /// <summary>
        /// Gets the tags.
        /// </summary>
        public IList<ProFormaTag> Tags { get; }
    }
}