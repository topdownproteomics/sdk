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
        /// <param name="neutronCount">The neutron count.</param>
        /// <param name="relativeAbundance">The relative abundance.</param>
        public Isotope(double atomicMass, int neutronCount, double relativeAbundance)
        {
            this.AtomicMass = atomicMass;
            this.NeutronCount = neutronCount;
            this.RelativeAbundance = relativeAbundance;
        }

        /// <summary>
        /// The atomic mass in daltons (Da).
        /// </summary>
        public double AtomicMass { get; }

        /// <summary>
        /// Gets the number of neutrons present in the atom's nucleus.
        /// </summary>
        public int NeutronCount { get; }
        
        /// <summary>
        /// The occurance of this isotope relative to others from a given element.
        /// </summary>
        /// <value>
        /// The abundance as a percentage from 0-1.
        /// </value>
        public double RelativeAbundance { get; }
    }
}