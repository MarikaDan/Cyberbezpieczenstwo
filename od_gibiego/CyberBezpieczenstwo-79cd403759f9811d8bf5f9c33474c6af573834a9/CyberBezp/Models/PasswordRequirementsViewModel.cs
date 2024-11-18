using System.ComponentModel.DataAnnotations;

namespace CyberBezp.Models
{
	public class PasswordRequirementsViewModel
	{
		[Display(Name = "Require Digit")]
		public bool RequireDigit { get; set; }

		[Display(Name = "Require Lowercase")]
		public bool RequireLowercase { get; set; }

		[Display(Name = "Require Non-Alphanumeric")]
		public bool RequireNonAlphanumeric { get; set; }

		[Display(Name = "Require Uppercase")]
		public bool RequireUppercase { get; set; }

		[Display(Name = "Required Length")]
		public int RequiredLength { get; set; }

		[Display(Name = "Required Unique Characters")]
		public int RequiredUniqueChars { get; set; }
	}
}