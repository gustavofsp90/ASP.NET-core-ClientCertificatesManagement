using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CertificatesManager.Models
{
	public class CertificateApplication
	{
		public int CertificateId { get; set; }
		public Certificate Certificate { get; set; }
		public int ApplicationId { get; set; }
		public Application Application { get; set; }
	}
}
