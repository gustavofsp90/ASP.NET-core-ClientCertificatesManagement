using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CertificatesManager.CustomAttributes;
using CertificatesManager.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CertificatesManager.ViewModels
{
	public class GroupViewModel
	{
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		[GroupCustomAttributes]
		public IEnumerable<string> UserIds { get; set; } = new List<string>();
		public List<SelectListItem> Users { get; set; }
	}
}
