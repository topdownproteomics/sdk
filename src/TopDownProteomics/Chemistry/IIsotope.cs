namespace TopDownProteomics.Chemistry
{
    /// <summary>
    /// Represents a single isotope of a chemical element. Contains a unique number
    /// of protons and neutrons compared to every other isotope.
    /// </summary>
    public interface IIsotope
    {
        /// <summary>
        /// The atomic mass in daltons (Da).
        /// </summary>
        double AtomicMass { get; }

        /// <summary>
        /// Gets the number of neutrons present in the atom's nucleus.
        /// </summary>
        int NeutronCount { get; }

        /// <summary>
        /// The occurance of this isotope relative to others from a given element.
        /// </summary>
        /// <value>
        /// The abundance as a percentage from 0-1.
        /// </value>
        double RelativeAbundance { get; }
    }
}