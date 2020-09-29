using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.IO.MzIdentML.Models
{
	/// <summary>
	/// Corresponds to the ProteinAmbiguityGroup element
	/// </summary>
	public class ProteinAmbiguityGroup
	{
		/// <summary>
		/// Gets and sets the id
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets and sets the protein detection hypotheses
		/// </summary>
		public List<ProteinDetectionHypothesis> ProteinDetectionHypotheses { get; set; }


	}
}
