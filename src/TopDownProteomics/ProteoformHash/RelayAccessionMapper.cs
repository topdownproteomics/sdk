using System;
using TopDownProteomics.ProForma;

namespace TopDownProteomics.ProteoformHash
{
    /// <summary>
    /// Maps a modification accession to a PSI-MOD accession.
    /// </summary>
    /// <seealso cref="IAccessionMapper" />
    public class RelayAccessionMapper : IAccessionMapper
    {
        private Func<string, Tuple<ProFormaEvidenceType, string>> _func;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayAccessionMapper"/> class.
        /// </summary>
        /// <param name="func">The function.</param>
        public RelayAccessionMapper(Func<string, Tuple<ProFormaEvidenceType, string>> func)
        {
            _func = func;
        }

        /// <summary>
        /// Maps the given accession to a PSI-MOD accession.
        /// </summary>
        /// <param name="accession">The accession.</param>
        /// <returns></returns>
        public Tuple<ProFormaEvidenceType, string> MapAccession(string accession)
        {
            return _func(accession);
        }
    }
}