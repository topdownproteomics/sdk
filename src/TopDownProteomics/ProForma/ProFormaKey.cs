namespace TopDownProteomics.ProForma
{
    ///// <summary>
    ///// Possible keys for a ProFormaDescriptor
    ///// </summary>
    //public enum ProFormaKey
    //{
    //    /// <summary>
    //    /// No key provided
    //    /// </summary>
    //    None = 0,

    //    /// <summary>
    //    /// The modification name comes from one of the default ontologies.
    //    /// </summary>
    //    KnownModificationName = 1,

    //    /// <summary>
    //    /// A delta mass of unknown annotation.
    //    /// </summary>
    //    Mass = 2,

    //    /// <summary>
    //    /// A chemical formula in our notation
    //    /// </summary>
    //    Formula = 3,

    //    /// <summary>
    //    /// The user defined extra information
    //    /// </summary>
    //    Info = 4,

    //    /// <summary>
    //    /// The Unimod database identifier
    //    /// </summary>
    //    Unimod = 11,

    //    /// <summary>
    //    /// The UniProt database identifier
    //    /// </summary>
    //    UniProt = 12,

    //    /// <summary>
    //    /// The RESID database identifier
    //    /// </summary>
    //    Resid = 13,

    //    /// <summary>
    //    /// The PSI-MOD database identifier
    //    /// </summary>
    //    PsiMod = 14,

    //    /// <summary>
    //    /// The BRNO identifier
    //    /// </summary>
    //    Brno = 15,

    //    /// <summary>
    //    /// The XL-MOD identifier
    //    /// </summary>
    //    XlMod = 16,

    //    /// <summary>
    //    /// The GNO identifier
    //    /// </summary>
    //    Gno = 17
    //}

    // Classes
    //  Ontology
    //  Theo or Observed
    //  Name, Accession, or Mass

    // What can you assign an ontology to? accession, mass, name ... formula? why not?

    /// <summary>Possible keys for a descriptor.</summary>
    public enum ProFormaKey
    {
        /// <summary>No key provided.</summary>
        None = 0,

        /// <summary>The exact modification name from an ontology.</summary>
        Name = 1,

        /// <summary>An identifier from an ontology.</summary>
        Identifier = 2,

        /// <summary>A delta mass of unknown annotation.</summary>
        Mass = 3,

        /// <summary>A chemical formula in our notation.</summary>
        Formula = 4,

        /// <summary>A glycan composition in our notation.</summary>
        Glycan = 5,

        /// <summary>The user defined extra information.</summary>
        Info = 6,
    }

    /// <summary>Evidence types to provide on a descriptor, typically an ontology identifier.</summary>
    public enum ProFormaEvidenceType
    {
        /// <summary>No evidence provided</summary>
        None = 0,

        /// <summary>Experimentally observed evidence</summary>
        Observed = 1,

        /// <summary>The Unimod database identifier</summary>
        Unimod = 2,

        /// <summary>The UniProt database identifier</summary>
        UniProt = 3,

        /// <summary>The RESID database identifier</summary>
        Resid = 4,

        /// <summary>The PSI-MOD database identifier</summary>
        PsiMod = 5,

        /// <summary>The XL-MOD identifier</summary>
        XlMod = 6,

        /// <summary>The GNO identifier</summary>
        Gno = 7,

        /// <summary>The BRNO identifier (https://doi.org/10.1038/nsmb0205-110)</summary>
        Brno = 8
    }
}