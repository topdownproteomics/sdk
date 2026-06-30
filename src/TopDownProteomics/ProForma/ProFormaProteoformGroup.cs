using System.Collections.Generic;

namespace TopDownProteomics.ProForma
{
    /// <summary>
    /// A group of chimeric peptidoforms encoded in a single ProForma 2.0 string and joined by <c>+</c>
    /// (ProForma 2.0 section 7.2). A non-chimeric string yields a group containing one peptidoform.
    /// </summary>
    public class ProFormaProteoformGroup
    {
        /// <summary>Initializes a new instance of the <see cref="ProFormaProteoformGroup"/> class.</summary>
        /// <param name="peptidoforms">The chimeric peptidoforms, in order.</param>
        public ProFormaProteoformGroup(IList<ProFormaPeptidoform> peptidoforms)
        {
            this.Peptidoforms = peptidoforms;
        }

        /// <summary>The chimeric peptidoforms, in order; joined by <c>+</c> when more than one.</summary>
        public IList<ProFormaPeptidoform> Peptidoforms { get; }
    }
}
