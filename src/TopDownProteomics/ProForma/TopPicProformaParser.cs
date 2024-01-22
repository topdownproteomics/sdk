using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TopDownProteomics.Chemistry;
using TopDownProteomics.Chemistry.Unimod;
using TopDownProteomics.IO.Unimod;
using TopDownProteomics.ProForma.Validation;

namespace TopDownProteomics.ProForma
{
    /// <summary>
    /// A parser for TopPic strings into a ProformaTerm <see cref="ProFormaTerm"/>
    /// </summary>
    public class TopPicProformaParser
    {
        IDictionary<string, ProFormaDescriptor>? _modLookup = null;

        #region Regex strings
        Regex _modRx = new Regex(@"\(([A-Z]{1,})\)(\[[\w_+-.]+\])+");
        Regex _numberRx = new Regex(@"(-?\+?[0-9]+.[0-9]+)");
        Regex _terminalAaRx = new Regex(@"\P{N}(\.)\P{N}??|\P{N}??(\.)\P{N}");
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TopPicProformaParser"/> class.
        /// </summary>
        public TopPicProformaParser() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TopPicProformaParser"/> class.
        /// </summary>
        /// <param name="modFile">The mod.txt file for mapping modifications.</param>
        public TopPicProformaParser(string modFile)
        {
            _modLookup = ParseModFile(modFile);
        }

        /// <summary>
        /// Gets the proforma term.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        /// <returns></returns>
        public ProFormaTerm ParseTopPicString(string sequence)
        {
            //first remove terminaltags if there!
            sequence = RemoveTerminalAAs(sequence);
            var ptms = FindPTMs(sequence);
            return new ProFormaTerm(GetFullyStrippedSequence(sequence), ptms.Item3, ptms.Item1, ptms.Item2);
        }

        private IDictionary<string, ProFormaDescriptor> ParseModFile(string modFile)
        {
            IDictionary<string, ProFormaDescriptor> modLookup = new Dictionary<string, ProFormaDescriptor>();

            using StreamReader reader = new StreamReader(modFile);

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if (line.Length == 0 | line.StartsWith("#"))
                    continue;

                //# To input a modification, use the following format:
                //# Name,Mass,Residues,Position,UnimodID

                var splitLine = line.Split(',');
                var name = splitLine[0];

                if (Int32.TryParse(splitLine[4], out var uniModNumber))
                {
                    if (uniModNumber > 0)
                        modLookup.Add(name, new ProFormaDescriptor(ProFormaKey.Identifier, ProFormaEvidenceType.Unimod, $"UNIMOD:{uniModNumber}"));
                    else if (uniModNumber == -1)
                        modLookup.Add(name, new ProFormaDescriptor(ProFormaKey.None, name)); // maybe could do mass instead?
                    else
                        throw new Exception($"invalid unimod id");

                }
                else
                    throw new Exception($"Failed to parse unimod id {splitLine[1]}");
            }

            return modLookup;
        }


        private string GetFullyStrippedSequence(string sequence) => Regex.Replace(sequence, @"\[[\w_+-.]+\]|[()]", "");

        private Dictionary<int, int> GetIndexLoopup(string sequence)
        {
            Dictionary<int, int> indexLoopup = new Dictionary<int, int>();

            bool inBraket = false;
            int index = 0;
            for (int i = 0; i < sequence.Length; i++)
            {
                char c = sequence[i];
                if (c == '[')
                    inBraket = true;
                else if (c == ']')
                    inBraket = false;
                else if (char.IsUpper(c) && !inBraket)
                {
                    indexLoopup[i] = index++;
                }
            }
            return indexLoopup;
        }

        private Tuple<List<ProFormaDescriptor>, List<ProFormaDescriptor>, List<ProFormaTag>> FindPTMs(string sequence)
        {
            var indexLoopup = GetIndexLoopup(sequence);

            List<ProFormaDescriptor> N_terms = new List<ProFormaDescriptor>();
            List<ProFormaDescriptor> C_terms = new List<ProFormaDescriptor>();
            List<ProFormaTag> Tags = new List<ProFormaTag>();

            foreach (Match match in _modRx.Matches(sequence))
            {
                var startIndex = indexLoopup[match.Groups[1].Index];
                var ptms = match.Groups[2].Captures;

                if (ptms.Count > 1)
                    throw new Exception("multiple mods are not currently accepeted");

                if (startIndex == 0 && match.Groups[1].Length == 1)  // check for ambiguoous mods that include the start -> just make tags
                {
                    N_terms = ParsePTMs(ptms);
                }
                else if (startIndex == indexLoopup.Max(x => x.Value))
                {
                    C_terms = ParsePTMs(ptms);
                }
                else if (match.Groups[1].Length > 1)
                {
                    var EndIndex = startIndex + match.Groups[1].Length - 1;
                    Tags.Add(new ProFormaTag(startIndex, EndIndex, ParsePTMs(ptms)));
                }
                else

                    Tags.Add(new ProFormaTag(startIndex, ParsePTMs(ptms)));

            }
            return new Tuple<List<ProFormaDescriptor>, List<ProFormaDescriptor>, List<ProFormaTag>>(N_terms, C_terms, Tags);
        }

        private List<ProFormaDescriptor> ParsePTMs(CaptureCollection ptms)
        {
            var proformaList = new List<ProFormaDescriptor>();

            foreach (var ptm in ptms)
                proformaList.Add(ParsePTMstring(ptm.ToString()));

            return proformaList;
        }

        private ProFormaDescriptor ParsePTMstring(string ptmstring)
        {
            //strip []
            ptmstring = ptmstring.Substring(1, ptmstring.Length - 2);
            var numberMatch = _numberRx.Match(ptmstring);

            if (numberMatch.Success)
                return new ProFormaDescriptor(ProFormaKey.Mass, numberMatch.Value);

            // Find and throw exception if there is a *
            if (ptmstring.Contains('*'))
                throw new Exception("multiple mods are not currently supported");

            if (_modLookup is not null && _modLookup.ContainsKey(ptmstring))
                return _modLookup[ptmstring];
            else
                return new ProFormaDescriptor(ptmstring);
        }

        private string RemoveTerminalAAs(string sequence)
        {
            var matches = _terminalAaRx.Matches(sequence);

            if (matches.Count > 0)
            {
                var startIndex = matches[0].Groups[1].Index + 1;
                var length = matches[1].Groups[1].Index - startIndex;
                sequence = sequence.Substring(startIndex, length);
            }
            return sequence;
        }
    }
}