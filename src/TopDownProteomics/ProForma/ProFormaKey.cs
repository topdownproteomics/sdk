using System;

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

    // Classes
    //  Ontology
    //  Theo or Observed
    //  Name, Accession, or Mass

    // What can you assign an ontology to? accession, mass, name ... formula? why not?

    internal enum ProFormaDescriptorType
    {
        /// <summary>
        /// No key provided
        /// </summary>
        None = 0,

        /// <summary>
        /// The modification name comes from one of the default ontologies.
        /// </summary>
        Name = 1,

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
    }

    internal enum ProFormaOntology
    {
        /// <summary>
        /// No key provided
        /// </summary>
        None = 0,

        /// <summary>
        /// The Unimod database identifier
        /// </summary>
        Unimod = 1,

        /// <summary>
        /// The UniProt database identifier
        /// </summary>
        UniProt = 2,

        /// <summary>
        /// The RESID database identifier
        /// </summary>
        Resid = 3,

        /// <summary>
        /// The PSI-MOD database identifier
        /// </summary>
        PsiMod = 4,

        /// <summary>
        /// The BRNO identifier
        /// </summary>
        Brno = 5,

        /// <summary>
        /// The XL-MOD identifier
        /// </summary>
        XlMod = 6,

        /// <summary>
        /// The GNO identifier
        /// </summary>
        Gno = 7
    }

    /// <summary>A flag based enum to mark multiple attributes on a descriptor.</summary>
    [Flags]
    public enum ProFormaKey2
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