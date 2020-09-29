using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.IO.MzIdentML.Models
{
	/// <summary>
	/// Contract for CvParams
	/// </summary>
	public interface ICvParam: IUserParam
	{
		/// <summary>
		/// Gets and sets the Accession
		/// </summary>
		string Accession { get; set; }
	}
}
