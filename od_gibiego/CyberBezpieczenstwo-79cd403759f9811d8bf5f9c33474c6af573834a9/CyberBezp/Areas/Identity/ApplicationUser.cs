using Microsoft.AspNetCore.Identity;

namespace CyberBezp.Areas.Identity;

public class ApplicationUser : IdentityUser
{
    public List<string> PreviousPasswords { get; set; } = [];
    public DateTime LastPasswordChangeDate { get; set; }
    public int PasswordValidForDays { get; set; } = 90;
    public int SecurityFunctionArgumentA { get; set; } = 1;

    public override string? PasswordHash
    {
        get => base.PasswordHash;
        set
        {
            if (value is null)
                return;

            base.PasswordHash = value;

            PreviousPasswords.Add(value);
            LastPasswordChangeDate = DateTime.Now;
        }
    }

    public bool IsPasswordExpired()
    {
        return LastPasswordChangeDate.AddDays(PasswordValidForDays) < DateTime.Now;
    }

}