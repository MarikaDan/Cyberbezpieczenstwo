using CyberBezp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Options;

namespace CyberBezp.Areas.Identity
{
    public class ApplicationSignInManager : SignInManager<ApplicationUser>
    {
        private readonly ISystemLogger _logger;
        public ApplicationSignInManager(UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<ApplicationUser>> logger, IAuthenticationSchemeProvider schemes, IUserConfirmation<ApplicationUser> confirmation, ISystemLogger sysLogger) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
            _logger = sysLogger;
        }
        private static double SecretOneWayFunction(int a, int x)
        {
            //tg(a*x)
            return Math.Round(Math.Tan(a*x), 2);
        }

        public Task<SignInResult> PasswordAndSecurityFunctionSignInAsync(ApplicationUser user, string password, int securityFunctionX, double securityFunctionAnswer, bool isPersistent, bool lockoutOnFailure)
        {

            var a = user.SecurityFunctionArgumentA;
            var correctAnswer = SecretOneWayFunction(a, securityFunctionX);

            if (Math.Abs(correctAnswer - securityFunctionAnswer) > 0.0001)
                return Task.FromResult(SignInResult.Failed);

            return PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }

        public override Task SignOutAsync()
        {
            var user = UserManager.GetUserAsync(Context.User).Result;
            _logger.Log("Wylogowanie", "User logged out.");
            return base.SignOutAsync();
        }
    }
}
