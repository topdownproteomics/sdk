using System.Collections.Generic;

namespace TopDownProteomics.ProForma
{
    /// <summary>A tag that is spread across multiple distinct sites.</summary>
    public class ProFormaTagGroup : IProFormaDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProFormaTagGroup"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="members">The members.</param>
        /// <param name="preferredLocalization">The zero-based member index for which member is the preferred localization.</param>
        public ProFormaTagGroup(string name, ProFormaKey key, string value, IList<ProFormaMembershipDescriptor> members, int preferredLocalization)
            : this(name, key, ProFormaEvidenceType.None, value, members, preferredLocalization) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProFormaTagGroup" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="key">The key.</param>
        /// <param name="evidenceType">Type of the evidence.</param>
        /// <param name="value">The value.</param>
        /// <param name="members">The members.</param>
        /// <param name="preferredLocalization">The zero-based member index for which member is the preferred localization.</param>
        public ProFormaTagGroup(string name, ProFormaKey key, ProFormaEvidenceType evidenceType, string value,
            IList<ProFormaMembershipDescriptor> members, int preferredLocalization)
        {
            this.Name = name;
            this.Key = key;
            this.EvidenceType = evidenceType;
            this.Value = value;
            this.Members = members;
            this.PreferredLocalization = preferredLocalization;
        }

        /// <summary>The name of the group.</summary>
        public string Name { get; }

        /// <summary>The key.</summary>
        public ProFormaKey Key { get; internal set; }

        /// <summary>The type of the evidence.</summary>
        public ProFormaEvidenceType EvidenceType { get; internal set; }

        /// <summary>The value.</summary>
        public string Value { get; internal set; }

        /// <summary>The members of the group.</summary>
        public IList<ProFormaMembershipDescriptor> Members { get; }

        /// <summary>The</summary>
        public int PreferredLocalization { get; private set; }

        /// <summary>
        /// Used to assign the preferred localizaiton while members are being added.
        /// </summary>
        /// <param name="zeroBasedMemberIndex"></param>
        public void AssignPreferredLocalization(int zeroBasedMemberIndex)
        {
            PreferredLocalization = zeroBasedMemberIndex;
        }
    }
}