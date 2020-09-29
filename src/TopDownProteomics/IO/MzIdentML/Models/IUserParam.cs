using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.IO.MzIdentML.Models
{
	public interface IUserParam
	{
		string Name { get; set; }
		string Value { get; set; }
	}
}
