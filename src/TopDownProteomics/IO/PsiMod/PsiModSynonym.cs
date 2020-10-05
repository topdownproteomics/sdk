namespace TopDownProteomics.IO.PsiMod
{
    /// <summary>PsiMod Synonym</summary>
    public class PsiModSynonym
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PsiModSynonym"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="text">The text.</param>
        /// <param name="scope">The scope.</param>
        public PsiModSynonym(string type, string text, string scope)
        {
            Type = type;
            Text = text;
            Scope = scope;
        }

        /// <summary>The synonym type.</summary>
        public string Type { get; }

        /// <summary>The synonym text.</summary>
        public string Text { get; }

        /// <summary>The synonym scope.</summary>
        public string Scope { get; }
    }
}