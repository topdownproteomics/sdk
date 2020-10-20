using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the ProteinDetectionList element
	/// </summary>
	public class MzIdentMlProteinDetectionList
	{
		/// <summary>
		/// Instantiates with schema-required parameters
		/// </summary>
		/// <param name="id"></param>
		public MzIdentMlProteinDetectionList(string id)
		{
			this.Id = id;
		}

		/// <summary>
		/// Gets the id
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Gets and sets the name
		/// </summary>
		public string? Name { get; set; }

		/// <summary>
		/// Gets and sets the protein ambiguity groups
		/// </summary>
		public List<MzIdentMlProteinAmbiguityGroup>? ProteinAmbiguityGroups { get; set; }

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
