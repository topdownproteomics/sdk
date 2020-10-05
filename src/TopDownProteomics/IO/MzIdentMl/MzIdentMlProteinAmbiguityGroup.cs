using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the ProteinAmbiguityGroup element
	/// </summary>
	public class MzIdentMlProteinAmbiguityGroup
	{
		/// <summary>
		/// Gets and sets the id
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets and sets the protein detection hypotheses
		/// </summary>
		public List<MzIdentMlProteinDetectionHypothesis> ProteinDetectionHypotheses { get; set; }


	}
}
