using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to Inputs element
	/// </summary>
	public class MzIdentMlInputs
	{
		/// <summary>
		/// Instantiates with required spectra data
		/// </summary>
		/// <param name="spectraData"></param>
		public MzIdentMlInputs(MzIdentMlSpectraData spectraData)
		{
			this.SpectraData = spectraData;
		}

		/// <summary>
		/// Gets the spectradata
		/// </summary>
		public MzIdentMlSpectraData SpectraData { get; }

		/// <summary>
		/// Gets and sets the sourcefiles
		/// </summary>
		public List<MzIdentMlSourceFile>? SourceFiles { get; set; }

		/// <summary>
		/// Gets and sets the search databases
		/// </summary>
		public List<MzIdentMlSearchDatabase>? SearchDatabases { get; set; }
	}
}
