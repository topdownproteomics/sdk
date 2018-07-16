using System;
using System.Collections.Generic;
using System.Linq;
using TopDownProteomics.IO.Obo;

namespace TopDownProteomics.IO.Unimod
{
    /// <summary>
    /// Unimod obo parser
    /// </summary>
    public class UnimodOboParser
    {
        /// <summary>
        /// Parses the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public IEnumerable<UnimodModification> Parse(string path)
        {
            var oboParser = new OboParser();

            return oboParser.Parse(path).Select(term => this.ConvertToModification(term));
        }

        /// <summary>
        /// Parses the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public IEnumerable<UnimodModification> ParseText(string text)
        {
            var oboParser = new OboParser();

            return oboParser.ParseText(text).Select(term => this.ConvertToModification(term));
        }

        private UnimodModification ConvertToModification(OboTerm term)
        {
            string code = term.Id;
            int id = Convert.ToInt32(code.Substring(7));
            string name = term.Name;

            string definition = null;
            string diffFormula = null;
            double diffMonoMass = 0;
            double diffAvMass = 0;

            foreach (OboTagValuePair pair in term.ValuePairs)
            {
                if (pair.Tag == "def")
                    definition = pair.Value.Replace("\\", string.Empty);
                else if (pair.Tag == "xref" && pair.Value.StartsWith("delta_composition"))
                    diffFormula = pair.Value.Replace("\"", string.Empty).Substring(18);
                else if (pair.Tag == "xref" && pair.Value.StartsWith("delta_mono_mass"))
                    diffMonoMass = Convert.ToDouble(pair.Value.Replace("\"", string.Empty).Substring(16));
                else if (pair.Tag == "xref" && pair.Value.StartsWith("delta_avge_mass"))
                    diffAvMass = Convert.ToDouble(pair.Value.Replace("\"", string.Empty).Substring(16));
            }

            return new UnimodModification(id, name, definition, diffFormula, diffMonoMass, diffAvMass);
        }
    }
}