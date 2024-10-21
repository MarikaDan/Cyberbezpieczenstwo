using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.Drawing.Text;

namespace Lab2.Models
{
    public class UserManager : UserManager<User>
    {
        public UserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {

        }

        public override async Task<IdentityResult>ChangePasswordAsync(string userId, string currentPassword, string newPasword)
        {
            if(await IsPreviousPassword(userId, newPasword))
            {
                return await Task.FromResult(IdentityResult.Failed("Nie mozna zmienic hasla");
            }
            var result = await base.ChangePasswordAsync(userId, currentPassword, newPasword);
            if (result.Succeeded)
            {
                var store = Store as UserStore;
                await store.AddToPreviousPasswordAsync(await FindByIdAsync(userId), PasswordHasher.HashPassword(newPassword));
            }
            return result;
        }
        public override async Task <IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            if(await IsPrevious)
        }
    }
}
