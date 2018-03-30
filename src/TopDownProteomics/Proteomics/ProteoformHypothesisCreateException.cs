using System;

namespace TopDownProteomics.Proteomics
{
    /// <summary>
    /// Base ProForma parsing exception.
    /// </summary>
    /// <seealso cref="Exception" />
    public class ProteoformHypothesisCreateException
        : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProteoformHypothesisCreateException"/> class.
        /// </summary>
        public ProteoformHypothesisCreateException()
            : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProteoformHypothesisCreateException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ProteoformHypothesisCreateException(string message)
            : base(message) { }
    }
}