using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CertificatesManager.CustomAttributes
{
	public class GroupCustomAttributes : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			List<string> users = ((IEnumerable<string>)value).ToList();
			if (users.Count == 0)
				return new ValidationResult("Please select a user.");
			return ValidationResult.Success;
		}
	}
}
