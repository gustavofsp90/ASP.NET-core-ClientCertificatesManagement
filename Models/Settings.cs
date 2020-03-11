using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CertificatesManager.CustomAttributes;

namespace CertificatesManager.Models
{
	public class Settings
	{
		public int Id { get; set; }
		[Required]
		public Group Group { get; set; }
		public string EmailSubject { get; set; }
		public int DaysBeforeEmail { get; set; }
	}
}
