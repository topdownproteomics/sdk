namespace TopDownProteomics.ProForma
{
    /// <summary>
    /// Possible keys for a ProFormaDescriptor
    /// </summary>
    public enum ProFormaKey
    {
        /// <summary>
        /// No key provided
        /// </summary>
        None = 0,

        /// <summary>
        /// The modification name comes from one of the default ontologies.
        /// </summary>
        KnownModificationName = 1,

        /// <summary>
        /// A delta mass of unknown annotation.
        /// </summary>
        Mass = 2,

        /// <summary>
        /// A chemical formula in our notation
        /// </summary>
        Formula = 3,

        /// <summary>
        /// The user defined extra information
        /// </summary>
        Info = 4,

        /// <summary>
        /// The Unimod database identifier
        /// </summary>
        Unimod = 11,

        /// <summary>
        /// The UniProt database identifier
        /// </summary>
        UniProt = 12,

        /// <summary>
        /// The RESID database identifier
        /// </summary>
        Resid = 13,

        /// <summary>
        /// The PSI-MOD database identifier
        /// </summary>
        PsiMod = 14,

        /// <summary>
        /// The BRNO identifier
        /// </summary>
        Brno = 15,

        /// <summary>
        /// The XL-MOD identifier
        /// </summary>
        XlMod = 16,

        /// <summary>
        /// The GNO identifier
        /// </summary>
        Gno = 17
    }
}