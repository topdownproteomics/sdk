namespace TopDownProteomics.ProForma
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
        /// Initializes a descriptor without value only
        /// </summary>
        /// <param name="value"></param>
        public ProFormaDescriptor(string value)
        {
            this.Key = ProFormaKey.Mod;
            this.Value = value;
        }

        /// <summary>The key.</summary>
        public string Key { get; }

        /// <summary>The value.</summary>
        public string Value { get; }

        /// <summary>
        /// String representation of <see cref="ProFormaDescriptor"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{Key}:{Value}";
    }
}