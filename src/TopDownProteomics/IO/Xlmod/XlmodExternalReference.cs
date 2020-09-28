namespace TopDownProteomics.IO.Xlmod
{
    /// <summary>XLMOD External Reference</summary>
    public class XlmodExternalReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XlmodExternalReference"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        public XlmodExternalReference(string name, string id)
        {
            Name = name;
            Id = id;
        }

        /// <summary>The reference name.</summary>
        public string Name { get; }

        /// <summary>The identifier from the external reference.</summary>
        public string Id { get; }
    }
}