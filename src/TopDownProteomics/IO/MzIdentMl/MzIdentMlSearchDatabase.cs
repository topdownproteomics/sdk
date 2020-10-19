using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the SearchDatabase element
	/// </summary>
	public class MzIdentMlSearchDatabase
	{
		/// <summary>
		/// Instantiates with schema-required parameters
		/// </summary>
		/// <param name="id"></param>
		/// <param name="location"></param>
		/// <param name="fileFormat"></param>
		/// <param name="databaseName"></param>
		public MzIdentMlSearchDatabase(string id, string location, MzIdentMlCvParam fileFormat, MzIdentMlCvOrUserParam databaseName)
		{
			this.Id = id;
			this.Location = location;
			this.FileFormat = fileFormat;
			this.DatabaseName = databaseName;
		}

		/// <summary>
		/// Gets and sets the Id
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Gets and sets the location
		/// </summary>
		public string Location { get; }

		/// <summary>
		/// Gets and sets the database name
		/// </summary>
		public MzIdentMlCvOrUserParam DatabaseName { get; }

		/// <summary>
		/// Gets and sets the name
		/// </summary>
		public string? Name { get; set; }

		/// <summary>
		/// Gets and sets the number of sequences
		/// </summary>
		public int? NumberOfSequences { get; set; }

		/// <summary>
		/// Gets and sets the number of residues
		/// </summary>
		public int? NumberOfResidues { get; set; }

		/// <summary>
		/// Gets and sets the release date
		/// </summary>
		public string? ReleaseDate { get; set; }

		/// <summary>
		/// Gets and sets the version info
		/// </summary>
		public string? Version { get; set; }

		/// <summary>
		/// Gets and sets the FileFormat
		/// </summary>
		public MzIdentMlCvParam? FileFormat { get; set; }

		/// <summary>
		/// Gets and sets the cvParams
		/// </summary>
		public List<MzIdentMlCvParam>? CvParams { get; set; }
	}
}
