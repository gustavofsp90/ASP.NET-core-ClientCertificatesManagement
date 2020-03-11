using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CertificatesManager.Models.Enum
{
	public enum Environment
	{
		Development = 1,
		[Display(Name = "Pre-Production")]
		PreProduction = 2,
		Production = 3,
		Other = 4
	}
}
