using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CertificatesManager.CustomAttributes;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CertificatesManager.ViewModels
{
	public class SettingsViewModel
	{
		public int Id { get; set; }
		[Required]
		public string Group { get; set; }
		public List<SelectListItem> Groups { get; set; }
		public string EmailSubject { get; set; }
		public int DaysBeforeEmail { get; set; }
	}
}
