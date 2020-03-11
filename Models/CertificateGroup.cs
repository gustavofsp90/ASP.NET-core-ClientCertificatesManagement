using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CertificatesManager.Models
{
	public class CertificateGroup
	{
		public int CertificateId { get; set; }
		public Certificate Certificate { get; set; }
		public int GroupId { get; set; }
		public Group Group { get; set; }
	}
}
