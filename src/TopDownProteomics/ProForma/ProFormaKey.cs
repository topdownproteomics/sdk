namespace TopDownProteomics.ProForma
{
    /// <summary>
    /// Possible keys for a ProFormaDescriptor
    /// </summary>
    public class ProFormaKey
    {
        /// <summary>
        /// The modification name
        /// </summary>
        public static string Mod { get; } = "mod";

        /// <summary>
        /// The Unimod database identifier
        /// </summary>
        public static string Unimod { get; } = "Unimod";

        /// <summary>
        /// The UniProt database identifier
        /// </summary>
        public static string UniProt { get; } = "UniProt";

        /// <summary>
        /// The RESID database identifier
        /// </summary>
        public static string Resid { get; } = "RESID";

        /// <summary>
        /// The PSI-MOD database identifier
        /// </summary>
        public static string PsiMod { get; } = "PSI-MOD";

        /// <summary>
        /// The BRNO identifier
        /// </summary>
        public static string Brno { get; } = "BRNO";

        /// <summary>
        /// The mass
        /// </summary>
        public static string Mass { get; } = "mass";

        /// <summary>
        /// The formula (in Unimod notation)
        /// </summary>
        public static string Formula { get; } = "formula";

        /// <summary>
        /// The user defined extra information
        /// </summary>
        public static string Info { get; } = "info";
    }
}