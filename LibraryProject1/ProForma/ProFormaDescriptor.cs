namespace TestLibNamespace.ProForma
{
    /// <summary>
    /// Member of the tag. Could be a key-value pair, or a keyless entry.
    /// </summary>
    public class ProFormaDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProFormaDescriptor"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public ProFormaDescriptor(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public string Value { get; }
    }
}