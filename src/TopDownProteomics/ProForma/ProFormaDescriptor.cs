using System;

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
            if (Enum.TryParse(key, true, out ProFormaKey parsedKey))
                this.Key = parsedKey;
            else
                throw new ProFormaParseException("The key " + key + " is not supported.");

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

        /// <summary>
        /// Gets the key.
        /// </summary>
        public ProFormaKey Key { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public string Value { get; }
    }
}