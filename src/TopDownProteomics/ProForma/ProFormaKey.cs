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
        public static string Mod = "mod";

        /// <summary>
        /// The Unimod database identifier
        /// </summary>
        public static string Unimod = "Unimod";

        /// <summary>
        /// The RESID database identifier
        /// </summary>
        public static string Resid = "RESID";

        /// <summary>
        /// The PSI-MOD database identifier
        /// </summary>
        public static string PsiMod = "PSI-MOD";

        /// <summary>
        /// The BRNO identifier
        /// </summary>
        public static string Brno = "BRNO";

        /// <summary>
        /// The mass
        /// </summary>
        public static string Mass = "mass";

        /// <summary>
        /// The formula (in Unimod notation)
        /// </summary>
        public static string Formula = "formula";

        /// <summary>
        /// The user defined extra information
        /// </summary>
        public static string Info = "info";
    }
}