using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the IonType element
	/// </summary>
	public class MzIdentMlIonType
	{
		/// <summary>
		/// Gets and sets the indices
		/// </summary>
		public int[] Indices { get; set; }

		/// <summary>
		/// Gets and sets the charge
		/// </summary>
		public int Charge { get; set; }

		/// <summary>
		/// Gets and sets the m/z's
		/// </summary>
		public double[] Mzs { get; set; }

		/// <summary>
		/// Gets and sets the intensities
		/// </summary>
		public double[] Intensities { get; set; }

		/// <summary>
		/// Gets and sets the m/z error
		/// </summary>
		public double[] MzError { get; set; }

		/// <summary>
		/// Gets and sets the parameters
		/// </summary>
		public List<MzIdentMlParam> Parameters { get; set; } = new List<MzIdentMlParam>();
	}
}
