using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to SourceFile elements
	/// </summary>
	public class MzIdentMlSourceFile
	{
		/// <summary>
		/// Instantiates with required parameters
		/// </summary>
		/// <param name="id"></param>
		/// <param name="location"></param>
		public MzIdentMlSourceFile(string id, string location)
		{
			this.Id = id;
			this.Location = location;
		}

		/// <summary>
		/// Gets and sets the id
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Gets and sets the location
		/// </summary>
		public string Location { get; }

		/// <summary>
		/// Gets and sets the cvParams
		/// </summary>
		public List<MzIdentMlCvParam>? CvParams { get; set; }

		/// <summary>
		/// Gets and sets the user params
		/// </summary>
		public List<MzIdentMlUserParam>? UserParams { get; set; }

		/// <summary>
		/// Gets and sets the file format
		/// </summary>
		public MzIdentMlCvParam? FileFormat { get; set; }
	}
}
