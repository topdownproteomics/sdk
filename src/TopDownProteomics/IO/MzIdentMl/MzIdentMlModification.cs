namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the Modification element
	/// </summary>
	public class MzIdentMlModification
	{
		/// <summary>
		/// Instantiates with required accession, name, and reference
		/// </summary>
		/// <param name="accession"></param>
		/// <param name="name"></param>
		/// <param name="reference"></param>
		public MzIdentMlModification(string accession, string name, string reference)
		{
			this.Accession = accession;
			this.Name = name;
			this.Reference = reference;
		}
		/// <summary>
		/// Gets and sets the location
		/// </summary>
		public int? Location { get; set; }

		/// <summary>
		/// Gets and sets the monoisotopic mass delta
		/// </summary>
		public double? MonoisotopicMassDelta { get; set; }

		/// <summary>
		/// Gets and sets the accession
		/// </summary>
		public string Accession { get; }

		/// <summary>
		/// Gets and sets the name
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets and sets the reference
		/// </summary>
		public string Reference { get; }
	}
}
