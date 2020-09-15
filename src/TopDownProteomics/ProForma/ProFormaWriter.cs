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

            // Check unlocalized modifications
            if (term.UnlocalizedTags != null && term.UnlocalizedTags.Count > 0)
            {
                foreach (var tag in term.UnlocalizedTags)
                {
                    if (tag.Descriptors != null && tag.Descriptors.Count > 0)
                        sb.Append($"[{this.CreateDescriptorText(tag.Descriptors)}]?");
                }
            }

            // Check N-terminal modifications
            if (term.NTerminalDescriptors != null && term.NTerminalDescriptors.Count > 0)
            {
                sb.Append($"[{this.CreateDescriptorText(term.NTerminalDescriptors)}]-");
            }

            // Check indexed modifications
            if (term.Tags != null)
            {
                int startIndex = 0;
                foreach (var tag in term.Tags.OrderBy(x => x.ZeroBasedStartIndex))
                {
                    // Write sequence up to tag
                    sb.Append(term.Sequence.Substring(startIndex, tag.ZeroBasedStartIndex - startIndex + 1));
                    startIndex = tag.ZeroBasedStartIndex + 1;

                    // Write the tag
                    sb.Append($"[{this.CreateDescriptorText(tag.Descriptors)}]");
                }

                // Write the rest of the sequence
                sb.Append(term.Sequence.Substring(startIndex));
            }
            else
            {
                sb.Append(term.Sequence);
            }

            // Check C-terminal modifications
            if (term.CTerminalDescriptors != null && term.CTerminalDescriptors.Count > 0)
            {
                sb.Append($"-[{this.CreateDescriptorText(term.CTerminalDescriptors)}]");
            }

            return sb.ToString();
        }

        private string CreateDescriptorText(IList<ProFormaDescriptor> descriptors)
        {
            return string.Join("|", descriptors.Select(x => $"{x}"));
        }
    }
}