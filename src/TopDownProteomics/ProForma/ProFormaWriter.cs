using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDownProteomics.ProForma
{
    /// <summary>
    /// Writer for the ProForma proteoform notation
    /// </summary>
    public class ProFormaWriter
    {
        /// <summary>
        /// Writes the string.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns></returns>
        public string WriteString(ProFormaTerm term)
        {
            var sb = new StringBuilder();

            // Check global modifications
            if (term.GlobalModifications != null)
            {
                foreach (var globalMod in term.GlobalModifications)
                {
                    if (globalMod.TargetAminoAcids != null)
                        sb.Append($"<[{this.CreateDescriptorsText(globalMod.Descriptors)}]@{string.Join(',', globalMod.TargetAminoAcids)}>");
                    else
                        sb.Append($"<{this.CreateDescriptorsText(globalMod.Descriptors)}>");
                }
            }

            // Check labile modifications
            if (term.LabileDescriptors != null)
            {
                sb.Append($"{{{this.CreateDescriptorsText(term.LabileDescriptors)}}}");
            }

            // Check unlocalized modifications
            if (term.UnlocalizedTags != null && term.UnlocalizedTags.Count > 0)
            {
                foreach (var tag in term.UnlocalizedTags)
                {
                    if (tag.Descriptors != null && tag.Descriptors.Count > 0)
                        sb.Append($"[{this.CreateDescriptorsText(tag.Descriptors)}]");

                    if (tag.Count != 1)
                        sb.Append($"^{tag.Count}");
                }

                // Only write out a single question mark
                sb.Append('?');
            }

            // Check N-terminal modifications
            if (term.NTerminalDescriptors != null && term.NTerminalDescriptors.Count > 0)
            {
                sb.Append($"[{this.CreateDescriptorsText(term.NTerminalDescriptors)}]-");
            }

            var tagsAndGroups = new List<(object, int, int, bool, double)>();

            if (term.Tags != null)
                tagsAndGroups.AddRange(term.Tags.Select(x => ValueTuple.Create((object)x, x.ZeroBasedStartIndex, x.ZeroBasedEndIndex, true, 0.0)));

            if (term.TagGroups != null)
                tagsAndGroups.AddRange(term.TagGroups
                    .SelectMany(x => x.Members
                    .Select(member => ValueTuple.Create((object)x, member.ZeroBasedStartIndex, member.ZeroBasedEndIndex, member == x.Members[x.PreferredLocalization], member.Weight))));

            //Remove any 

            // Check indexed modifications
            if (tagsAndGroups.Count > 0)
            {
                // Sort by start index
                tagsAndGroups.Sort((x, y) => x.Item2.CompareTo(y.Item2));

                int currentIndex = 0;
                foreach (var (obj, startIndex, endIndex, displayValue, weight) in tagsAndGroups)
                {
                    if (startIndex == endIndex)
                    {
                        // Write sequence up to tag
                        sb.Append(term.Sequence.Substring(currentIndex, startIndex - currentIndex + 1));
                        currentIndex = startIndex + 1;
                    }
                    else // Handle ambiguity range
                    {
                        // Write sequence up to range
                        sb.Append(term.Sequence.Substring(currentIndex, startIndex - currentIndex));

                        // Write sequence in range
                        //if ambiguous sequence, don't add parenthesis because they're already in the sequence
                        if (term.AmbiguousAASequences != null && term.AmbiguousAASequences.Any(x => x.ZeroBasedStartIndex == startIndex && x.ZeroBasedEndIndex == endIndex))
                        {
                            sb.Append($"{term.Sequence.Substring(startIndex, endIndex - startIndex + 1)}");
                        }
                        else
                        {
                            sb.Append($"({term.Sequence.Substring(startIndex, endIndex - startIndex + 1)})");
                        }
                        currentIndex = endIndex + 1;
                    }

                    if (obj is ProFormaTag tag)
                    {
                        sb.Append($"[{this.CreateDescriptorsText(tag.Descriptors)}]");
                    }
                    else if (obj is ProFormaTagGroup group)
                    {
                        if (displayValue)
                            sb.Append($"[{this.CreateDescriptorText(group)}#{group.Name}");
                        else
                            sb.Append($"[#{group.Name}");

                        if (weight > 0.0)
                            sb.Append($"({weight})]");
                        else
                            sb.Append(']');
                    }
                }

                // Write the rest of the sequence
                sb.Append(term.Sequence.Substring(currentIndex));
            }
            else
            {
                sb.Append(term.Sequence);
            }

            // Check C-terminal modifications
            if (term.CTerminalDescriptors != null && term.CTerminalDescriptors.Count > 0)
            {
                sb.Append($"-[{this.CreateDescriptorsText(term.CTerminalDescriptors)}]");
            }

            return sb.ToString();
        }

        private string CreateDescriptorsText(IList<ProFormaDescriptor> descriptors)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < descriptors.Count; i++)
            {
                sb.Append(this.CreateDescriptorText(descriptors[i]));

                if (i < descriptors.Count - 1)
                    sb.Append('|');
            }

            return sb.ToString();
        }

        private string CreateDescriptorText(IProFormaDescriptor descriptor)
        {
            return descriptor.Key switch
            {
                ProFormaKey.Formula => $"Formula:{descriptor.Value}",
                ProFormaKey.Glycan => $"Glycan:{descriptor.Value}",
                ProFormaKey.Info => $"Info:{descriptor.Value}",
                var x when x == ProFormaKey.Name || x == ProFormaKey.Mass => descriptor.EvidenceType switch
                {
                    ProFormaEvidenceType.None => descriptor.Value, // We assume the name is enough
                    ProFormaEvidenceType.Observed => $"Obs:{descriptor.Value}",
                    ProFormaEvidenceType.Unimod => $"U:{descriptor.Value}",
                    ProFormaEvidenceType.Resid => $"R:{descriptor.Value}",
                    ProFormaEvidenceType.PsiMod => $"M:{descriptor.Value}",
                    ProFormaEvidenceType.XlMod => $"X:{descriptor.Value}",
                    ProFormaEvidenceType.Gno => $"G:{descriptor.Value}",
                    ProFormaEvidenceType.Brno => $"B:{descriptor.Value}",
                    _ => throw new Exception($"Can't handle {descriptor.Key} with evidence type: {descriptor.EvidenceType}.")
                },
                ProFormaKey.Identifier => descriptor.EvidenceType switch
                {
                    ProFormaEvidenceType.Observed => $"Obs:{descriptor.Value}",
                    ProFormaEvidenceType.Resid => $"RESID:{descriptor.Value}",
                    _ => descriptor.Value
                },
                _ => descriptor.Value
            };
        }
    }
}