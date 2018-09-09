namespace TopDownProteomics.ProForma
{
    /// <summary>
    /// Possible keys describing range of ambiguous PTM assignment.
    /// </summary>
    public class ProFormaAmbiguityAffix
    {
        /// <summary>
        /// Marks a possible site at which a modification may be localized
        /// </summary>
        public static string PossibleSite { get; } = "#";

        /// <summary>
        /// Marks the left boundary of the range over which a modification may be localized.
        /// </summary>
        public static string LeftBoundary { get; } = "->";

        /// <summary>
        /// Marks the right boundary of the range over which a modification may be localized.
        /// </summary>
        public static string RightBoundary { get; } = "<-";

        /// <summary>
        /// Marks a modification that is not localized
        /// </summary>
        public static string Unlocalized { get; } = "<->";
    }
}