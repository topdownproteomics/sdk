namespace TopDownProteomics.Chemistry.Unimod
{
    /// <summary>
    /// The number of this particular atom that are contained in the composition.
    /// </summary>
    public class UnimodCompositionAtomCardinality
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnimodCompositionAtomCardinality"/> class.
        /// </summary>
        /// <param name="atom">The atom.</param>
        /// <param name="count">The count.</param>
        public UnimodCompositionAtomCardinality(UnimodCompositionAtom atom, int count)
        {
            this.Atom = atom;
            this.Count = count;
        }

        /// <summary>
        /// Gets the atom.
        /// </summary>
        public UnimodCompositionAtom Atom { get; }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count { get; }
    }
}