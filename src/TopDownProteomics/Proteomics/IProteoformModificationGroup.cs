using System.Collections.Generic;

namespace TopDownProteomics.Proteomics
{
    /// <summary>A modification that is applied to multiple possible places on the proteoform.</summary>
    public interface IProteoformModificationGroup
    {
        /// <summary>The name of the modification group.</summary>
        string GroupName { get; }

        /// <summary>The members of the group.</summary>
        IReadOnlyCollection<IProteoformModificationGroupMember> Members { get; }
    }
}