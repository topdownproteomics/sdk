using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.ProteoformHash
{
    /// <summary>A chemical proteoform hash.</summary>
    public interface IChemicalProteoformHash
    {
        /// <summary>Gets the chemical proteoform hash.</summary>
        /// <value>The hash.</value>
        string Hash { get; }
        /// <summary>Gets a value indicating whether this instance has a ProForma string.</summary>
        /// <value>
        ///   <c>true</c> if this instance has a ProForma string; otherwise, <c>false</c>.</value>
        bool HasProForma { get; }
        /// <summary>Gets the ProForma string.</summary>
        /// <returns>The ProForma string or null if not created.</returns>
        string ProForma { get; }
    }
}
