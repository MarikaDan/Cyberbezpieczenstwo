using System.ComponentModel.DataAnnotations;

namespace Lab2.ViewModels
{
    public class AddUserViewModel
    {

        [Microsoft.Build.Framework.Required]
        [EmailAddress]
        public string Email { get; set; }

        [Microsoft.Build.Framework.Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Microsoft.Build.Framework.Required]
        public string Role { get; set; }

        [Microsoft.Build.Framework.Required]
        public int SecretFunctionParameterA { get; set; }
    }
}