using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CertificatesManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CertificatesManager.ViewModels
{
	public class CertificateViewModel
	{
		public int Id { get; set; }
		[Required]
		public IFormFile File { get; set; }
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
		public IEnumerable<string> ApplicationIds { get; set; } = new List<string>();
		public List<SelectListItem> Applications { get; set; }
		public IEnumerable<string> ServerIds { get; set; } = new List<string>();
		public List<SelectListItem> Servers { get; set; }
		public IEnumerable<string> GroupIds { get; set; } = new List<string>();
		public List<SelectListItem> Groups { get; set; }
		public string Purpose { get; set; }
		public Models.Enum.Environment Environment { get; set; }
		public DateTime From { get; set; }
		public DateTime To { get; set; }
		public string InstallationLink { get; set; }
		public string OtherInfo { get; set; }
		public bool Active { get; set; }
	}
}
