using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDownProteomics.ProForma;

namespace TopDownProteomics.IO.MzIdentML.Models
{
    /// <summary>
    /// Corresponds to the Peptide element
    /// </summary>
    public class Peptide
    {
        /// <summary>
        /// Gets and sets the id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets and sets the sequence
        /// </summary>
        public string Sequence { get; set; }

        /// <summary>
        /// Gets and sets the modifications
        /// </summary>
        public List<Modification> Modifications { get; set; } = new List<Modification>();

        /// <summary>
        /// Gets and sets the proforma string
        /// </summary>
        public string ProForma { get; set; }

        /// <summary>
        /// Gets and sets the biological proteoform record number
        /// </summary>
        public int BiologicalProteoformRecordNumber { get; set; }

        /// <summary>
        /// Gets and sets the chemical proteoform record number
        /// </summary>
        public int ChemicalProteoformRecordNumber { get; set; }

        /// <summary>
        /// Creates a proforma string
        /// </summary>
        public void CreateProForma()
        {
            var nTermMods = new List<ProFormaDescriptor>();
            var internalMods = new List<ProFormaTag>();
            var cTermMods = new List<ProFormaDescriptor>();

            foreach (var mod in this.Modifications)
            {
                switch (mod.GetModLocationType(this.Sequence.Length))
                {
                    case ModLocationType.NTerminal:
                        nTermMods.Add(mod.ProFormaDescriptor);
                        break;
                    case ModLocationType.Internal:
                        internalMods.Add(mod.ProFormaTag);
                        break;
                    case ModLocationType.CTerminal:
                        cTermMods.Add(mod.ProFormaDescriptor);
                        break;
                    default:
                        break;
                }
            }

            var term = new ProFormaTerm(this.Sequence, unlocalizedTags:null, nTermMods, cTermMods, internalMods);
            this.ProForma = new ProFormaWriter().WriteString(term);
        }




	}
}
