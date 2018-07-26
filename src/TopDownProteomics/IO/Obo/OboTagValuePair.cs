namespace TopDownProteomics.IO.Obo
{
    /// <summary>
    /// An Obo pair of a tag and its value.
    /// </summary>
    public class OboTagValuePair
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OboTagValuePair" /> class.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="value">The value.</param>
        public OboTagValuePair(string tag, string value)
        {
            this.Tag = tag;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }
    }
}