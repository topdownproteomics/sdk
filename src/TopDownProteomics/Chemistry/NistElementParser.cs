using System;
using System.Collections.Generic;
using System.IO;

namespace TopDownProteomics.Chemistry
{
    /// <summary>
    /// Parses elements from here: https://physics.nist.gov/cgi-bin/Compositions/stand_alone.pl?ele=&amp;ascii=ascii2&amp;isotype=some
    /// </summary>
    public class NistElementParser
    {
        /// <summary>
        /// Parses the text.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public IList<IElement> ParseFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            List<IElement> elements = new List<IElement>();
            int currentAtomNumber = -1;
            string currentSymbol = null;
            List<IIsotope> currentIsotopes = null;
            double currentRelativeAtomicMass = 0.0;
            double currentIsotopicComposition = 0.0;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("Atomic Number = "))
                {
                    int atomicNumber = Convert.ToInt32(lines[i].Substring(lines[i].LastIndexOf('=') + 1));

                    if (currentAtomNumber > 0 && atomicNumber > currentAtomNumber && currentIsotopes != null)
                    {
                        // Add new element
                        elements.Add(new Element(currentAtomNumber, currentSymbol, currentIsotopes));

                        currentSymbol = null; // Needed so Hydrogen gets the right symbol
                        currentIsotopes = null;
                    }

                    currentAtomNumber = atomicNumber;
                }
                else if (lines[i].StartsWith("Atomic Symbol = ") && currentSymbol == null)
                {
                    currentSymbol = lines[i].Substring(lines[i].LastIndexOf('=') + 1).Trim();
                }
                else if (lines[i].StartsWith("Relative Atomic Mass = "))
                {
                    int equalSign = lines[i].LastIndexOf('=');
                    int openParen = lines[i].IndexOf('(');

                    currentRelativeAtomicMass = Convert.ToDouble(lines[i].Substring(equalSign + 1, openParen - equalSign - 1));
                }
                else if (lines[i].StartsWith("Isotopic Composition = "))
                {
                    int equalSign = lines[i].LastIndexOf('=');
                    int openParen = lines[i].IndexOf('(');

                    if (openParen < 0)
                    {
                        if (lines[i].Length > equalSign + 2 && lines[i][equalSign + 2] == '1')
                            currentIsotopicComposition = 1.0;
                        else
                            continue; // No known composition, skip line
                    }
                    else
                        currentIsotopicComposition = Convert.ToDouble(lines[i].Substring(equalSign + 1, openParen - equalSign - 1));

                    // Add new isotope to collection
                    if (currentIsotopes == null)
                        currentIsotopes = new List<IIsotope>();

                    currentIsotopes.Add(new Isotope(currentRelativeAtomicMass, currentIsotopicComposition));
                }
            }

            // Add last element
            if (currentIsotopes != null)
                elements.Add(new Element(currentAtomNumber, currentSymbol, currentIsotopes));

            return elements;
        }
    }
}