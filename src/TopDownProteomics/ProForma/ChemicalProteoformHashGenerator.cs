using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDownProteomics.Chemistry;
using TopDownProteomics.Proteomics;

namespace TopDownProteomics.ProForma
{
    /// <summary>Creates a chemical proteoform hash from a proteoform group.</summary>
    public class ChemicalProteoformHashGenerator
    {
        /// <summary>Generates a chemical proteoform hash for the specified proteoform group.</summary>
        /// <param name="proteoformGroup">The proteoform group.</param>
        /// <returns></returns>
        public string Generate(IProteoformGroup proteoformGroup)
        {
            ProFormaDescriptor nTermDescriptor =
                this.GetFormulaDescriptor(proteoformGroup.NTerminalModification);
            IList<ProFormaDescriptor> nTermDescriptors = nTermDescriptor == null ? null : new[] { nTermDescriptor };

            ProFormaDescriptor cTermDescriptor =
                this.GetFormulaDescriptor(proteoformGroup.CTerminalModification);
            IList<ProFormaDescriptor> cTermDescriptors = cTermDescriptor == null ? null : new[] { cTermDescriptor };

            IList<ProFormaTag> tags = new List<ProFormaTag>();
            if (proteoformGroup.Modifications != null)
            {
                foreach (IProteoformModificationWithIndex proteoformModificationWithIndex in proteoformGroup.Modifications)
                {
                    ProFormaDescriptor descriptor = this.GetFormulaDescriptor(proteoformModificationWithIndex);
                    tags.Add(new ProFormaTag(proteoformModificationWithIndex.ZeroBasedIndex, new[] { descriptor }));
                }
            }

            ProFormaTerm proFormaTerm = new ProFormaTerm(proteoformGroup.GetSequence(), null, nTermDescriptors, cTermDescriptors, tags.OrderBy(t => t.Descriptors.First().Value).ToArray());
            string hash = new ProFormaWriter().WriteString(proFormaTerm);
            return hash;
        }

        private ProFormaDescriptor GetFormulaDescriptor(IProteoformModification proteoformModification)
        {
            return proteoformModification == null
                    ? null
                    : new ProFormaDescriptor(ProFormaKey.Formula, this.GetChemicalFormulaString(proteoformModification.GetChemicalFormula()));
        }

        private string GetChemicalFormulaString(IChemicalFormula chemicalFormula)
        {
            ICollection<IEntityCardinality<IElement>> elements = chemicalFormula.GetElements().ToList();
            StringBuilder formula = new StringBuilder();
            IEntityCardinality<IElement> carbon = elements.SingleOrDefault(e => e.Entity.AtomicNumber == 6);

            if (carbon != null && carbon.Count > 0)
            {
                this.AppendFormula(formula, carbon);
                elements.Remove(carbon);
                IEntityCardinality<IElement> hydrogen = elements.SingleOrDefault(e => e.Entity.AtomicNumber == 1);
                if (hydrogen != null && hydrogen.Count > 0)
                {
                    this.AppendFormula(formula, hydrogen);
                    elements.Remove(hydrogen);
                }
            }
            
            foreach (IEntityCardinality<IElement> element in elements.OrderBy(e => e.Entity.Symbol))
            {
                if (element.Count > 0)
                {
                    this.AppendFormula(formula, element);
                }
            }

            return formula.ToString();
        }

        private void AppendFormula(StringBuilder formula, IEntityCardinality<IElement> element)
        {
            formula.Append($"{element.Entity.Symbol}{element.Count}");
        }
    }
}
