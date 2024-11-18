using System.ComponentModel.DataAnnotations;

namespace CyberBezp.Models
{
	public class AddUserViewModel
	{
		[Microsoft.Build.Framework.Required]
		[Display(Name = "Username")]
		public string UserName { get; set; }

		[Microsoft.Build.Framework.Required]
		[EmailAddress]
		public string Email { get; set; }

		[Microsoft.Build.Framework.Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Microsoft.Build.Framework.Required]
		public string Role { get; set; }
	}
}