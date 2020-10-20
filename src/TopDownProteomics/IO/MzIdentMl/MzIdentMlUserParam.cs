namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the UserParam element
	/// </summary>
	public class MzIdentMlUserParam
	{
		/// <summary>
		/// Instantiates with required parameters
		/// </summary>
		/// <param name="name"></param>
		public MzIdentMlUserParam(string name)
		{
			this.Name = name;
		}

		/// <summary>
		/// Gets the name
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets and sets the type
		/// </summary>
		public string? Type { get; set; }

		/// <summary>
		/// Gets and sets the unit accession
		/// </summary>
		public string? UnitAccession { get; set; }

		/// <summary>
		/// Gets and sets the unit CV reference
		/// </summary>
		public string? UnitCvRef { get; set; }

		/// <summary>
		/// Gets and sets the unit name
		/// </summary>
		public string? UnitName { get; set; }

		/// <summary>
		/// Gets and sets the value
		/// </summary>
		public string? Value { get; set; }
	}
}
