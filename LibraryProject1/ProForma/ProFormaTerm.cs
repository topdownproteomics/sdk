using System.Collections.Generic;

namespace TestLibNamespace.ProForma
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
        /// <param name="tags">The tags.</param>
        public ProFormaTerm(string sequence, ICollection<ProFormaTag> tags)
        {
            Sequence = sequence;
            Tags = tags;
        }

        /// <summary>
        /// Gets the amino acid sequence.
        /// </summary>
        public string Sequence { get; }

        /// <summary>
        /// Gets the tags.
        /// </summary>
        public ICollection<ProFormaTag> Tags { get; }
    }
}