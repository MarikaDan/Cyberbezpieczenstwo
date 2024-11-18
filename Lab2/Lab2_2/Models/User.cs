﻿using Microsoft.AspNetCore.Identity;

namespace Lab2_2.Models
{
    public class User : IdentityUser
    {
        public User() : base()
        {
        }
        public DateTime LastPasswordChange { get; set; }
        public virtual List<string> PreviousUserPasswords { get; set; } = [];

        public override string? PasswordHash { get => base.PasswordHash;
            set
            {
                if (value is null)
                    return;
               base.PasswordHash = value;
               PreviousUserPasswords.Add(value);
               LastPasswordChange = DateTime.Now;
            }
        }

        public int PasswordValidForDays { get; set; }

	}

}
