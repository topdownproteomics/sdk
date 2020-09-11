namespace TopDownProteomics.ProForma
{
    /// <summary>
    /// Member of the tag. Could be a key-value pair, or a keyless entry.
    /// </summary>
    public class ProFormaDescriptor
    {
        /// <summary>
        /// Initializes a descriptor without value only
        /// </summary>
        /// <param name="value"></param>
        public ProFormaDescriptor(string value)
            : this(ProFormaKey.Name, ProFormaEvidenceType.None, value) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProFormaDescriptor"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public ProFormaDescriptor(ProFormaKey key, string value)
            : this(key, ProFormaEvidenceType.None, value) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProFormaDescriptor" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="evidenceType">Type of the evidence.</param>
        /// <param name="value">The value.</param>
        public ProFormaDescriptor(ProFormaKey key, ProFormaEvidenceType evidenceType, string value)
        {
            this.Key = key;
            this.EvidenceType = evidenceType;
            this.Value = value;
        }

        /// <summary>The key.</summary>
        public ProFormaKey Key { get; }

        /// <summary>The type of the evidence.</summary>
        public ProFormaEvidenceType EvidenceType { get; }

        /// <summary>The value.</summary>
        public string Value { get; }

        /// <summary>String representation of <see cref="ProFormaDescriptor"/></summary>
        /// <returns></returns>
        public override string ToString() => $"{Key.ToString().ToLower()}:{Value}";
    }
}