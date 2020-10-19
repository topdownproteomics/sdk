using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the ProteinAmbiguityGroup element
	/// </summary>
	public class MzIdentMlProteinAmbiguityGroup
	{
		/// <summary>
		/// Instantiates with schema-required parameters
		/// </summary>
		/// <param name="id"></param>
		/// <param name="proteinDetectionHypotheses"></param>
		public MzIdentMlProteinAmbiguityGroup(string id, List<MzIdentMlProteinDetectionHypothesis> proteinDetectionHypotheses)
		{
			this.Id = id;
			this.ProteinDetectionHypotheses = proteinDetectionHypotheses;
		}

		/// <summary>
		/// Gets the id
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Gets and sets the protein detection hypotheses
		/// </summary>
		public List<MzIdentMlProteinDetectionHypothesis> ProteinDetectionHypotheses { get; }

		/// <summary>
		/// Gets and sets the cvParams
		/// </summary>
		public List<MzIdentMlCvParam>? CvParams { get; set; }

		/// <summary>
		/// Gets and sets the userParams
		/// </summary>
		public List<MzIdentMlUserParam>? UserParams { get; set; }


	}
}
