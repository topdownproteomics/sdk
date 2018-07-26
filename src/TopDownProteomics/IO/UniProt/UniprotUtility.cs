namespace TopDownProteomics.IO.UniProt
{
    /// <summary>
    /// Utility to get UniProt feature from description
    /// </summary>
    internal class UniprotUtility
    {
        public static UniprotFeatureType GetFeatureFromKeyName(string keyName)
        {
            switch (keyName)
            {
                case "ACT_SITE":
                case "active site":
                    return UniprotFeatureType.Activity;
                case "BINDING":
                case "binding site":
                    return UniprotFeatureType.Binding;
                case "CA_BIND":
                case "calcium-binding region":
                    return UniprotFeatureType.CalciumBinding;
                case "CARBOHYD":
                case "glycosylation site":
                    return UniprotFeatureType.Carbohydrate;
                case "CHAIN":
                case "chain":
                    return UniprotFeatureType.Chain;
                case "COILED":
                case "coiled-coil region":
                    return UniprotFeatureType.Coiled;
                case "COMPBIAS":
                case "compositionally biased region":
                    return UniprotFeatureType.CompositionallyBiased;
                case "CONFLICT":
                case "sequence conflict":
                    return UniprotFeatureType.SequenceConflict;
                case "CROSSLNK":
                case "cross-link":
                    return UniprotFeatureType.CrossLink;
                case "DISULFID":
                case "disulfide bond":
                    return UniprotFeatureType.DisulfideBond;
                case "DNA_BIND":
                case "DNA-binding region":
                    return UniprotFeatureType.DNABinding;
                case "DOMAIN":
                case "domain":
                    return UniprotFeatureType.Domain;
                case "HELIX":
                case "helix":
                    return UniprotFeatureType.Helix;
                case "INIT_MET":
                case "initiator methionine":
                    return UniprotFeatureType.InitialMethionine;
                case "INTRAMEM":
                case "intramembrane region":
                    return UniprotFeatureType.IntraMembrane;
                case "LIPID":
                case "lipid moiety-binding region":
                    return UniprotFeatureType.Lipid;
                case "METAL":
                case "metal ion-binding site":
                    return UniprotFeatureType.MetalIon;
                case "MOD_RES":
                case "modified residue":
                    return UniprotFeatureType.ModifiedResidue;
                case "MOTIF":
                case "short sequence motif":
                    return UniprotFeatureType.ShortMotif;
                case "MUTAGEN":
                case "mutagenesis site":
                    return UniprotFeatureType.Mutagenesis;
                case "NON_CONS":
                case "non-consecutive residues":
                    return UniprotFeatureType.NonConsecutiveResidues;
                case "NON_STD":
                case "non-standard amino acid":
                    return UniprotFeatureType.Nonstandard;
                case "NON_TER":
                case "non-terminal residue":
                    return UniprotFeatureType.NonTerminalResidues;
                case "NP_BIND":
                case "nucleotide phosphate-binding region":
                    return UniprotFeatureType.PhosphateBinding;
                case "PEPTIDE":
                case "peptide":
                    return UniprotFeatureType.Peptide;
                case "PROPEP":
                case "propeptide":
                    return UniprotFeatureType.Propep;
                case "REGION":
                case "region of interest":
                    return UniprotFeatureType.Region;
                case "REPEAT":
                case "Repeat":
                case "repeat":
                    return UniprotFeatureType.Repeat;
                case "SIGNAL":
                case "signal peptide":
                    return UniprotFeatureType.Signal;
                case "SITE":
                case "site":
                    return UniprotFeatureType.Site;
                case "STRAND":
                case "strand":
                    return UniprotFeatureType.Strand;
                case "TOPO_DOM":
                case "topological domain":
                    return UniprotFeatureType.TopologicalDomain;
                case "TRANSIT":
                case "transit peptide":
                    return UniprotFeatureType.Transit;
                case "TRANSMEM":
                case "transmembrane region":
                    return UniprotFeatureType.TransmembraneRegion;
                case "TURN":
                case "turn":
                    return UniprotFeatureType.Turn;
                case "UNSURE":
                case "unsure residue":
                    return UniprotFeatureType.Unsure;
                case "VARIANT":
                case "sequence variant":
                    return UniprotFeatureType.SequenceVariant;
                case "VAR_SEQ":
                case "splice variant":
                    return UniprotFeatureType.SpliceVariant;
                case "ZN_FING":
                case "zinc finger region":
                    return UniprotFeatureType.ZincFinger;
                default:
                    return UniprotFeatureType.Unknown;
            }
        }
    }
}