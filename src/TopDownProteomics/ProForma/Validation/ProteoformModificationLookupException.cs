using System;

namespace TopDownProteomics.ProForma.Validation
{
    /// <summary>
    /// Someting went wrong when looking up a modification.
    /// </summary>
    /// <seealso cref="Exception" />
    public class ProteoformModificationLookupException
    : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProteoformModificationLookupException"/> class.
        /// </summary>
        public ProteoformModificationLookupException()
            : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProteoformModificationLookupException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ProteoformModificationLookupException(string message)
            : base(message) { }
    }
}