using System.Collections.Generic;

namespace TopDownProteomics.ProForma
{
    /// <summary>
    /// A single peptidoform above the level of one <see cref="ProFormaTerm"/>: one or more chains
    /// joined by <c>//</c> (inter-chain crosslinks and branches, ProForma 2.0 sections 4.2.3.2 and
    /// 4.2.4), together with an optional charge state and ion adducts (section 7.1).
    /// </summary>
    public class ProFormaPeptidoform
    {
        /// <summary>Initializes a new instance of the <see cref="ProFormaPeptidoform"/> class.</summary>
        /// <param name="chains">The chains, in order; joined by <c>//</c> when more than one.</param>
        /// <param name="charge">The charge state, or <c>null</c> when none is specified.</param>
        /// <param name="ionAdducts">The raw ion-adduct text (e.g. <c>+2Na+,+H+</c>), or <c>null</c>.</param>
        public ProFormaPeptidoform(IList<ProFormaTerm> chains, int? charge = null, string? ionAdducts = null)
        {
            this.Chains = chains;
            this.Charge = charge;
            this.IonAdducts = ionAdducts;
        }

        /// <summary>The chains of this peptidoform, in order.</summary>
        public IList<ProFormaTerm> Chains { get; }

        /// <summary>The charge state, or <c>null</c> when the peptidoform carries no <c>/z</c>.</summary>
        public int? Charge { get; }

        /// <summary>
        /// The raw ion-adduct text between the brackets following the charge (e.g. <c>+2Na+,+H+</c>
        /// in <c>/2[+2Na+,+H+]</c>), or <c>null</c> when none is present.
        /// </summary>
        public string? IonAdducts { get; }
    }
}
