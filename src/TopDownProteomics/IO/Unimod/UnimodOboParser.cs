﻿using System;
using System.Collections.Generic;
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
            return this.ConvertToModifications(oboParser.Parse(path));
        }

        /// <summary>
        /// Parses the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public IEnumerable<UnimodModification> ParseText(string text)
        {
            var oboParser = new OboParser();
            return this.ConvertToModifications(oboParser.ParseText(text));
        }

        private IList<UnimodModification> ConvertToModifications(IEnumerable<OboTerm> terms)
        {
            IList<UnimodModification> modifications = new List<UnimodModification>();
            foreach (OboTerm term in terms)
            {
                // Skip the first modification as it is not a real one
                if (term.Id != "UNIMOD:0")
                {
                    UnimodModification modification = this.ConvertToModification(term);

                    modifications.Add(modification);
                }
            }

            return modifications;
        }

        private UnimodModification ConvertToModification(OboTerm term)
        {
            string code = term.Id;
            string name = term.Name;

            string? definition = null;
            string? diffFormula = null;
            double diffMonoMass = 0;
            double diffAvMass = 0;

            if (term.ValuePairs != null)
            {
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
            }

            if (definition != null && diffFormula != null)
                return new UnimodModification(code, name, definition, diffFormula, diffMonoMass, diffAvMass);

            throw new Exception("Could not find required 'definition' field.");
        }
    }
}