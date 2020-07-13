namespace TopDownProteomics.IO.PsiMod
{
    /// <summary>PsiMod External Reference</summary>
    public class PsiModExternalReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PsiModExternalReference"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        public PsiModExternalReference(string name, string id)
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