namespace TopDownProteomics.IO.UniProt
{
    /// <summary>
    /// Emun of Types of UniProt Features
    /// </summary>
    public enum UniprotFeatureType
    {
        /// <summary>
        /// Unknown feature.
        /// </summary>
        Unknown,

        /// <summary>
        /// The initial methionine
        /// </summary>
        InitialMethionine, // INIT_MET

        /// <summary>
        /// Modified residue
        /// </summary>
        ModifiedResidue, // MOD_RES

        /// <summary>
        /// A sequence variant
        /// </summary>
        SequenceVariant, // VARIANT

        /// <summary>
        /// A sequence conflict
        /// </summary>
        SequenceConflict, // CONFLICT

        /// <summary>
        /// The splice variant
        /// </summary>
        SpliceVariant, // VAR_SEQ

        /// <summary>
        /// The lipid
        /// </summary>
        Lipid, // LIPID (lipid moiety-binding region)

        /// <summary>
        /// The carbohydrate
        /// </summary>
        Carbohydrate, // CARBOHYD

        /// <summary>
        /// The cross link
        /// </summary>
        CrossLink, // CROSSLNK (cross-link)

        /// <summary>
        /// Binding site for any chemical group (co-enzyme, prosthetic group, etc.).
        /// </summary>
        Binding,

        /// <summary>
        /// Signal peptide
        /// </summary>
        Signal,

        /// <summary>
        /// Propeptide
        /// </summary>
        Propep,

        /// <summary>
        /// Transit peptide
        /// </summary>
        Transit,

        /// <summary>
        /// Some kind of endogenous cleavage.
        /// </summary>
        Chain,

        /// <summary>
        /// Extent of a propeptide.
        /// </summary>
        Peptide,

        /// <summary>
        /// Extent of a transmembrane region.
        /// </summary>
        TransmembraneRegion,

        /// <summary>
        /// Extent of a region located in a membrane without crossing it.
        /// </summary>
        IntraMembrane,

        /// <summary>
        /// Extent of a domain, which is defined as a specific combination of secondary structures organized into a characteristic three-dimensional structure or fold.
        /// </summary>
        Domain,

        /// <summary>
        /// Extent of an internal sequence repetition.
        /// </summary>
        Repeat,

        /// <summary>
        /// Extent of a calcium-binding region.
        /// </summary>
        CalciumBinding,

        /// <summary>
        /// Extent of a zinc finger region.
        /// </summary>
        ZincFinger,

        /// <summary>
        /// Extent of a DNA-binding region.
        /// </summary>
        DNABinding,

        /// <summary>
        /// Extent of a nucleotide phosphate-binding region.
        /// </summary>
        PhosphateBinding,

        /// <summary>
        /// Extent of a region of interest in the sequence.
        /// </summary>
        Region,

        /// <summary>
        /// Extent of a coiled-coil region.
        /// </summary>
        Coiled,

        /// <summary>
        /// Short (up to 20 amino acids) sequence motif of biological interest.
        /// </summary>
        ShortMotif,

        /// <summary>
        /// Extent of a compositionally biased region.
        /// </summary>
        CompositionallyBiased,

        /// <summary>
        /// Amino acid(s) involved in the activity of an enzyme.
        /// </summary>
        Activity,

        /// <summary>
        /// Binding site for a metal ion.
        /// </summary>
        MetalIon,

        /// <summary>
        /// Any interesting single amino-acid site on the sequence, that is not defined by another feature key. It can also apply to an amino acid bond which is represented by the positions of the two flanking amino acids.
        /// </summary>
        Site,

        /// <summary>
        /// Non-standard amino acid
        /// </summary>
        Nonstandard,

        /// <summary>
        /// The topological domain
        /// </summary>
        TopologicalDomain,

        /// <summary>
        /// Disulfide bond
        /// </summary>
        DisulfideBond,

        /// <summary>
        /// Non-consecutive residues
        /// </summary>
        NonConsecutiveResidues,

        /// <summary>
        /// The residue at an extremity of the sequence is not the terminal residue.
        /// </summary>
        NonTerminalResidues,

        /// <summary>
        /// Tertiary structure is known experimentally contains helix. 
        /// </summary>
        Helix,

        /// <summary>
        /// Tertiary structure is known experimentally contains turn.
        /// </summary>
        Turn,

        /// <summary>
        /// Tertiary structure is known experimentally contains strand.
        /// </summary>
        Strand,

        /// <summary>
        /// Uncertainties in the sequence
        /// </summary>
        Unsure,

        /// <summary>
        /// The mutagenesis is a site which has been experimentally altered by mutagenesis.
        /// </summary>
        Mutagenesis
    }
}
