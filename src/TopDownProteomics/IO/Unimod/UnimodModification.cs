namespace TopDownProteomics.IO.Unimod
{
    /// <summary>
    /// A modification from the UniMod database.
    /// </summary>
    public class UnimodModification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnimodModification" /> class.
        /// </summary>
        /// <param name="id">The modification identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="deltaComposition">The delta composition.</param>
        /// <param name="deltaMonoisotopicMass">The delta monoisotopic mass.</param>
        /// <param name="deltaAverageMass">The delta average mass.</param>
        public UnimodModification(int id, string name, string definition, string deltaComposition, double deltaMonoisotopicMass, double deltaAverageMass)
        {
            this.Id = id;
            this.Name = name;
            this.Definition = definition;
            this.DeltaComposition = deltaComposition;
            this.DeltaMonoisotopicMass = deltaMonoisotopicMass;
            this.DeltaAverageMass = deltaAverageMass;
        }

        /// <summary>
        /// Gets the modification identifier.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the definition.
        /// </summary>
        public string Definition { get; }

        /// <summary>
        /// Gets the delta composition.
        /// </summary>
        public string DeltaComposition { get; }

        /// <summary>
        /// Gets the delta monoisotopic mass.
        /// </summary>
        public double DeltaMonoisotopicMass { get; }

        /// <summary>
        /// Gets the delta average mass.
        /// </summary>
        public double DeltaAverageMass { get; }
    }
}