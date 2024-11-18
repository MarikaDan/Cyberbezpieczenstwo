using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Lab2_2.Models
{
    public class UserManager : UserManager<User>
    {
        public UserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {

        }

        public override async Task<IdentityResult> ChangePasswordAsync(User user, string token, string newPasword)
        {
            if (IsPreviousPassword(user, newPasword))
            {
                return await Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "Nie mozna zmienic hasla" }));
            }
            return await base.ChangePasswordAsync(user, token, newPasword);
        }
        public override async Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword)
        {
            //if(await IsPrevious)

            return await base.ResetPasswordAsync(user, token, newPassword);
        }

        public static bool IsPreviousPassword(User user, string newPassword)
        {
            var passwordHasher = new PasswordHasher<User>();

            return user.PreviousUserPasswords
                .Distinct()
                .Any(existingPassword =>
                {
                    var result = passwordHasher.VerifyHashedPassword(user, existingPassword, newPassword);
                    return result == PasswordVerificationResult.Success;
                });
        }
    }
}
