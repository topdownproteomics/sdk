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
            OboTerm currentTerm = null;
            bool inTerm = false;
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                if (line == "[Term]")
                {
                    currentTerm = new OboTerm();
                    inTerm = true;
                }
                else if (inTerm)
                {
                    // Empty line means term is over
                    if (string.IsNullOrEmpty(line))
                    {
                        inTerm = false;
                        yield return currentTerm;
                    }
                    else
                    {
                        if (line.StartsWith("id: "))
                        {
                            currentTerm.Id = line.Substring(4);
                        }
                        else if (line.StartsWith("name: "))
                        {
                            currentTerm.Name = line.Substring(6);
                        }
                        else
                        {
                            int index = line.IndexOf(':');

                            currentTerm.ValuePairs.Add(new OboTagValuePair(
                                line.Substring(0, index), // Tag
                                line.Substring(index + 2))); // Value (+2 handles preceding space)
                        }
                    }
                }
            }
        }
    }
}