using System;
using System.Collections.Generic;
using System.IO;

namespace TopDownProteomics.IO.Obo
{
    // http://www.geneontology.org/GO.format.obo-1_2.shtml
    /// <summary>
    /// Parses the Open Biomedical Ontology (OBO) format.
    /// </summary>
    public class OboParser
    {
        /// <summary>
        /// Parses the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public IEnumerable<OboTerm> Parse(string path)
        {
            using (FileStream fs = File.OpenRead(path))
            {
                using (var reader = new StreamReader(fs))
                {
                    foreach (OboTerm term in ParseAux(reader))
                    {
                        yield return term;
                    }
                }
            }
        }

        /// <summary>
        /// Parses the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public IEnumerable<OboTerm> ParseText(string text)
        {
            using (var reader = new StringReader(text))
            {
                foreach (OboTerm term in ParseAux(reader))
                {
                    yield return term;
                }
            }
        }

        private IEnumerable<OboTerm> ParseAux(TextReader reader)
        {
            bool inTerm = false;
            string line;
            string? id = null;
            string? name = null;
            List<OboTagValuePair>? pairs = null;

            while ((line = reader.ReadLine()) != null)
            {
                if (line == "[Term]")
                {
                    id = null;
                    name = null;
                    pairs = null;
                    inTerm = true;
                }
                else if (inTerm)
                {
                    // Empty line means term is over
                    if (string.IsNullOrEmpty(line))
                    {
                        inTerm = false;

                        if (id == null || name == null)
                            throw new Exception("OBO Term must have both 'id' and 'name'.");

                        yield return new OboTerm(id, name, pairs);
                    }
                    else
                    {
                        if (line.StartsWith("id: "))
                            id = line.Substring(4);
                        else if (line.StartsWith("name: "))
                            name = line.Substring(6);
                        else
                        {
                            int index = line.IndexOf(':');

                            if (pairs == null)
                                pairs = new List<OboTagValuePair>();

                            pairs.Add(new OboTagValuePair(
                                line.Substring(0, index), // Tag
                                line.Substring(index + 2))); // Value (+2 handles preceding space)
                        }
                    }
                }
            }

            // Check for last term
            if (inTerm)
            {
                if (id == null || name == null)
                    throw new Exception("OBO Term must have both 'id' and 'name'.");

                yield return new OboTerm(id, name, pairs);
            }
        }
    }
}