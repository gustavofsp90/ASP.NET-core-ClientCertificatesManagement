using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CertificatesManager.Models
{
	public class Certificate
	{
		public int Id { get; set; }
		public string Guid { get; set; }
		public string Extension { get; set; }
		[Required]
		public string Name { get; set; }
		[DataType(DataType.Password)]
		public string Password { get; set; }
		public string SubjectName { get; set; }
		public string SerialNumber { get; set; }
		public string Issuer { get; set; }
		public string Location { get; set; }
		public ICollection<CertificateApplication> Applications { get; set; }
		public ICollection<CertificateServer> Servers { get; set; }
		public ICollection<CertificateGroup> Groups { get; set; }
		public string Purpose { get; set; }
		public Enum.Environment Environment { get; set; }
		public DateTime From { get; set; }
		public DateTime To { get; set; }
		public string InstallationLink { get; set; }
		public string OtherInfo { get; set; }
		public bool Active { get; set; }

	}
}
