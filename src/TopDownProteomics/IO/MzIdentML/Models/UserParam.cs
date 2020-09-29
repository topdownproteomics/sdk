using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.IO.MzIdentML.Models
{
	/// <summary>
	/// Corresponds to userParams elements
	/// </summary>
	public class UserParam : IUserParam
	{
		/// <summary>
		/// Gets and sets the names
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets and sets the value
		/// </summary>
		public string Value { get; set; }
	}
}
