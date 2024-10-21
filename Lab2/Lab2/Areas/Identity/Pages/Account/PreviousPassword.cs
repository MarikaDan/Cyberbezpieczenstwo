using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab2.Areas.Identity.Pages.Account
{
    public class PreviousPassword
    {
        public PreviousPassword()
        {
            CreateDate = DateTimeOffset.Now;
        }
        [Key, Column(Order = 0)]
        public string PasswordHash { get; set; }    
        public DateTimeOffset CreateDate { get; set; }
        [Key, Column(Order = 1)]
        public string UserId { get; set; }
        public virtual IdentityApplicationUser User {  get; set; } 
    }
}
