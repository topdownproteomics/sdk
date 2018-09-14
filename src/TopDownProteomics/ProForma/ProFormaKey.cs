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
        public static readonly string Mod = "mod";

        /// <summary>
        /// The Unimod database identifier
        /// </summary>
        public static readonly string Unimod = "Unimod";

        /// <summary>
        /// The UniProt database identifier
        /// </summary>
        public static readonly string UniProt = "UniProt";

        /// <summary>
        /// The RESID database identifier
        /// </summary>
        public static readonly string Resid = "RESID";

        /// <summary>
        /// The PSI-MOD database identifier
        /// </summary>
        public static readonly string PsiMod = "PSI-MOD";

        /// <summary>
        /// The BRNO identifier
        /// </summary>
        public static readonly string Brno = "BRNO";

        /// <summary>
        /// The mass
        /// </summary>
        public static readonly string Mass = "mass";

        /// <summary>
        /// The formula (in Unimod notation)
        /// </summary>
        public static readonly string Formula = "formula";

        /// <summary>
        /// The user defined extra information
        /// </summary>
        public static readonly string Info = "info";

        /// <summary>
        /// Determines whether a key is one of the allowed keys
        /// </summary>
        /// <param name="key">key in question</param>
        /// <returns>true if <paramref name="key"/> is one of the standard keys</returns>
        public static bool IsValidKey(string key)
        {
            return key == Mod
                || key == Unimod
                || key == UniProt
                || key == Resid
                || key == PsiMod
                || key == Brno
                || key == Mass
                || key == Formula
                || key == Info;
        }
    }
}