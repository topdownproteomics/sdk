namespace TopDownProteomics.IO.Xlmod
{
    /// <summary>XLMOD Synonym</summary>
    public class XlmodSynonym
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XlmodSynonym"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="text">The text.</param>
        public XlmodSynonym(string type, string text)
        {
            Type = type;
            Text = text;
        }

        /// <summary>The synonym type.</summary>
        public string Type { get; }

        /// <summary>The synonym text.</summary>
        public string Text { get; }
    }
}