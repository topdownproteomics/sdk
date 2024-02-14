using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TopDownProteomics.ProForma;

/// <summary>
/// A parser for TopPIC strings into a ProformaTerm <see cref="ProFormaTerm"/>
/// </summary>
public class TopPicProformaParser
{
    IDictionary<string, IList<ProFormaDescriptor>>? _modLookup = null;

    #region Regex strings
    Regex _modRx = new(@"\(([A-Z]{1,})\)(\[.+?\])+");
    Regex _numberRx = new(@"(-?\+?[0-9]+.[0-9]+)");
    Regex _terminalAaRx = new(@"\P{N}(\.)\P{N}??|\P{N}??(\.)\P{N}");
    Regex _strippedSequenceRx = new(@"\[.+?\]|[()]");
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
        _modLookup = ParseModFile(new FileInfo(modFile).OpenRead()); ;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TopPicProformaParser"/> class.
    /// </summary>
    /// <param name="modStream">The mod stream.</param>
    public TopPicProformaParser(Stream modStream)
    {
        _modLookup = ParseModFile(modStream);
    }

    /// <summary>
    /// Gets the proforma term.
    /// </summary>
    /// <param name="sequence">The sequence.</param>
    /// <returns></returns>
    public ProFormaTerm ParseTopPicString(string sequence)
    {
        //first remove terminal AA tags if there!
        sequence = RemoveTerminalAAs(sequence);
        var (nTerms, cTerms, tags) = FindPTMs(sequence);
        return new ProFormaTerm(GetFullyStrippedSequence(sequence), tags, nTerms, cTerms);
    }

    private IDictionary<string, IList<ProFormaDescriptor>> ParseModFile(Stream modStream)
    {
        IDictionary<string, IList<ProFormaDescriptor>> modLookup = new Dictionary<string, IList<ProFormaDescriptor>>();

        using StreamReader reader = new StreamReader(modStream);

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();

            if (string.IsNullOrWhiteSpace(line) | line.StartsWith("#"))
                continue;

            //# To input a modification, use the following format:
            //# Name,Mass,Residues,Position,UnimodID
            var splitLine = line.Split(',');

            if (splitLine.Length != 5)
                throw new TopPicParserException("Failed to parse mod file");

            var name = splitLine[0];

            if (int.TryParse(splitLine[4], out var uniModNumber))
            {
                if (uniModNumber > 0)
                    modLookup.Add(name, new List<ProFormaDescriptor>()
                    {
                        new ProFormaDescriptor(ProFormaKey.Identifier, ProFormaEvidenceType.Unimod, $"UNIMOD:{uniModNumber}"),
                        new ProFormaDescriptor(ProFormaKey.Info, name)
                    });
                else if (uniModNumber == -1 && double.TryParse(splitLine[1], out var mass))
                    modLookup.Add(name, new List<ProFormaDescriptor>()
                    {
                        new ProFormaDescriptor(ProFormaKey.Mass, $"{mass:+#.000000;-#.000000}"),
                        new ProFormaDescriptor(ProFormaKey.Info, name)
                    });
                else
                    throw new TopPicParserException($"invalid UniMod Id or mass");
            }
            else
                throw new TopPicParserException($"Failed to parse UniMod Id {splitLine[4]}".Trim());
        }
        return modLookup;
    }

    private string GetFullyStrippedSequence(string sequence) => _strippedSequenceRx.Replace(sequence, "");

    private Dictionary<int, int> GetIndexLookup(string sequence)
    {
        Dictionary<int, int> indexLookup = new Dictionary<int, int>();

        bool inBracket = false;
        int index = 0;
        for (int i = 0; i < sequence.Length; i++)
        {
            char c = sequence[i];
            if (c == '[')
                inBracket = true;
            else if (c == ']')
                inBracket = false;
            else if (char.IsUpper(c) && !inBracket)
            {
                indexLookup[i] = index++;
            }
        }
        return indexLookup;
    }

    private Tuple<IList<ProFormaDescriptor>, IList<ProFormaDescriptor>, IList<ProFormaTag>> FindPTMs(string sequence)
    {
        var indexLookup = GetIndexLookup(sequence);

        List<ProFormaDescriptor> nTerms = new List<ProFormaDescriptor>();
        List<ProFormaDescriptor> cTerms = new List<ProFormaDescriptor>();
        List<ProFormaTag> tags = new List<ProFormaTag>();

        foreach (Match match in _modRx.Matches(sequence))
        {
            var startIndex = indexLookup[match.Groups[1].Index];
            var ptms = match.Groups[2].Captures;

            if (ptms.Count > 1)
                throw new TopPicParserException("multiple mods are not currently supported");

            if (startIndex == 0 && match.Groups[1].Length == 1)  // check for ambiguous mods that include the start -> just make tags
            {
                nTerms = ParsePtms(ptms);
            }
            else if (startIndex == indexLookup.Max(x => x.Value))
            {
                cTerms = ParsePtms(ptms);
            }
            else if (match.Groups[1].Length > 1)
            {
                var EndIndex = startIndex + match.Groups[1].Length - 1;
                tags.Add(new ProFormaTag(startIndex, EndIndex, ParsePtms(ptms)));
            }
            else
                tags.Add(new ProFormaTag(startIndex, ParsePtms(ptms)));
        }
        return new Tuple<IList<ProFormaDescriptor>, IList<ProFormaDescriptor>, IList<ProFormaTag>>(nTerms, cTerms, tags);
    }

    private List<ProFormaDescriptor> ParsePtms(CaptureCollection ptms)
    {
        var proformaList = new List<ProFormaDescriptor>();

        foreach (var ptm in ptms)
            proformaList.AddRange(ParsePtmString(ptm.ToString()));

        return proformaList;
    }

    private IList<ProFormaDescriptor> ParsePtmString(string ptmString)
    {
        //strip []
        ptmString = ptmString.Substring(1, ptmString.Length - 2);
        var numberMatch = _numberRx.Match(ptmString);

        if (numberMatch.Success && Double.TryParse(numberMatch.Value, out double val))
            return new List<ProFormaDescriptor>() { new ProFormaDescriptor(ProFormaKey.Mass, $"{val:+#.0000;-#.0000;0}")};
        
        // Find and throw exception if there is a *
        if (ptmString.Contains('*'))
            throw new TopPicParserException("multiple mods are not currently supported");

        if (_modLookup?.ContainsKey(ptmString) == true)
            return _modLookup[ptmString];
        else
            return new List<ProFormaDescriptor>() { new ProFormaDescriptor(ptmString) };
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

/// <summary>
/// An exception for the TopPIC to ProForma parser.
/// </summary>
/// <seealso cref="System.Exception" />
public class TopPicParserException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TopPicParserException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public TopPicParserException(string message) : base(message) { }
}