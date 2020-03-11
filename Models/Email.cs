using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CertificatesManager.Models
{
	public class Email
	{
		public Certificate Certificate { get; set; }
		public User User { get; set; }
	}
}
