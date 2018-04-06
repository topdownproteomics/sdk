namespace TopDownProteomics.Chemistry
{
    /// <summary>
    /// Default implementation of an isotope.
    /// </summary>
    /// <seealso cref="IIsotope" />
    public class Isotope : IIsotope
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Isotope"/> class.
        /// </summary>
        /// <param name="atomicMass">The atomic mass.</param>
        /// <param name="relativeAbundance">The relative abundance.</param>
        public Isotope(double atomicMass, double relativeAbundance)
        {
            this.AtomicMass = atomicMass;
            this.RelativeAbundance = relativeAbundance;
        }

        /// <summary>
        /// The atomic mass in daltons (Da).
        /// </summary>
        public double AtomicMass { get; }

        /// <summary>
        /// The occurance of this isotope relative to others from a given element.
        /// </summary>
        /// <value>
        /// The abundance as a percentage from 0-1.
        /// </value>
        public double RelativeAbundance { get; }
    }
}