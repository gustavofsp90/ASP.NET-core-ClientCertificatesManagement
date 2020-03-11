using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CertificatesManager.Models
{
	public class CertificateServer
	{
			public int CertificateId { get; set; }
			public Certificate Certificate { get; set; }
			public int ServerId { get; set; }
			public Server Server { get; set; }
	}
}
