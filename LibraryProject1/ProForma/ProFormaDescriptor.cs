using System;

namespace TestLibNamespace.ProForma
{
    /// <summary>
    /// Member of the tag. Could be a key-value pair, or a keyless entry.
    /// </summary>
    public class ProFormaDescriptor
    {

        #region Constructors

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
            this.Key = ProFormaKey.MOD;
            this.Value = value;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// Gets the key.
        /// </summary>
        public ProFormaKey Key { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public string Value { get; }

        #endregion Public Properties

        #region Constants

        /// <summary>
        /// Possible keys for a ProFormaDescriptor
        /// </summary>
        public enum ProFormaKey
        {
            MOD,
            MASS,
            FORMULA,
            INFO
        }

        #endregion Constants

    }
}