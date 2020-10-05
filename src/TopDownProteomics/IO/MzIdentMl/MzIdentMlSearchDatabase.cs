using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the SearchDatabase element
	/// </summary>
	public class MzIdentMlSearchDatabase
	{
		/// <summary>
		/// Gets and sets the Id
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets and sets the location
		/// </summary>
		public string Location { get; set; }

		/// <summary>
		/// Gets and sets the name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets and sets the number of sequences
		/// </summary>
		public int NumberOfSequences { get; set; }

		/// <summary>
		/// Gets and sets the number of residues
		/// </summary>
		public int NumberOfResidues { get; set; }

		/// <summary>
		/// Gets and sets the version info
		/// </summary>
		public string Version { get; set; }

		/// <summary>
		/// Gets and sets the file format
		/// </summary>
		public MzIdentMlParam FileFormat { get; set; }

		/// <summary>
		/// Gets and sets the database name
		/// </summary>
		public MzIdentMlParam DatabaseName { get; set; }

		/// <summary>
		/// Gets and sets database params
		/// </summary>
		public List<MzIdentMlParam> DatabaseParams { get; set; } = new List<MzIdentMlParam>();
	}
}
