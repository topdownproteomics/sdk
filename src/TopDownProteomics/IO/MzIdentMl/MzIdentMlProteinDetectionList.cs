using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the ProteinDetectionList element
	/// </summary>
	public class MzIdentMlProteinDetectionList
	{
		/// <summary>
		/// Gets and sets the id
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets and sets the protein ambiguity groups
		/// </summary>
		public List<MzIdentMlProteinAmbiguityGroup> ProteinAmbiguityGroups { get; set; }
	}
}
