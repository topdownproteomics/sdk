using System;

namespace TopDownProteomics.Proteomics
{
    /// <summary>
    /// Proteoform Group creation exception.
    /// </summary>
    /// <seealso cref="Exception" />
    public class ProteoformGroupCreateException
        : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProteoformGroupCreateException"/> class.
        /// </summary>
        public ProteoformGroupCreateException()
            : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProteoformGroupCreateException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ProteoformGroupCreateException(string message)
            : base(message) { }
    }
}