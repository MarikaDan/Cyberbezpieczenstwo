// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using CyberBezp.Services;

namespace CyberBezp.Areas.Identity.Pages.Account
{
	public class LoginModel : PageModel
	{
		private readonly ApplicationSignInManager _signInManager;
		private readonly ILogger<LoginModel> _logger;
		private readonly ISystemLogger _systemLogger;

		public LoginModel(ApplicationSignInManager signInManager, ILogger<LoginModel> logger, ISystemLogger systemLogger)
		{
			_signInManager = signInManager;
			_logger = logger;
			_systemLogger = systemLogger;
		}

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// 
        [BindProperty]
        public int SecurityFunctionArgument { get; set; }
        
		[BindProperty]
		public InputModel Input { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public IList<AuthenticationScheme> ExternalLogins { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public string ReturnUrl { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		[TempData]
		public string ErrorMessage { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public class InputModel
		{
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            /// 
            [HiddenInput]
            public int SecurityFunctionArgument { get; set; }
            [Required]
            [Display(Name = "Security Function Answer")]
            public double SecurityFunctionAnswer { get; set; }
            [Required]
			public string UserName { get; set; }

			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[Required]
			[DataType(DataType.Password)]
			public string Password { get; set; }

			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[Display(Name = "Remember me?")]
			public bool RememberMe { get; set; }
		}

		public async Task OnGetAsync(string returnUrl = null)
		{
			if (!string.IsNullOrEmpty(ErrorMessage))
			{
				ModelState.AddModelError(string.Empty, ErrorMessage);
			}

			returnUrl ??= Url.Content("~/");

			// Clear the existing external cookie to ensure a clean login process
			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            var random = new Random();
            SecurityFunctionArgument = random.Next(1, 100);

            ReturnUrl = returnUrl;
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");

			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			if (ModelState.IsValid)
			{
				var user = await _signInManager.UserManager.FindByNameAsync(Input.UserName);
				if (user != null)
				{
                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                    var result = await _signInManager.PasswordAndSecurityFunctionSignInAsync(user, Input.Password, Input.SecurityFunctionArgument, Input.SecurityFunctionAnswer, Input.RememberMe,
                        lockoutOnFailure: true);
                    if (result.Succeeded)
					{
						_logger.LogInformation("User logged in.");
						_systemLogger.Log("Udane logowanie", $"{user.UserName} zalogowano");
						return LocalRedirect(returnUrl);
					}

					if (result.RequiresTwoFactor)
					{
						return RedirectToPage("./LoginWith2fa",
							new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
					}

					if (result.IsLockedOut)
					{
						_logger.LogWarning("User account locked out.");
						return RedirectToPage("./Lockout");
					}
					else
					{
						ModelState.AddModelError(string.Empty, "Invalid username or password");
						return Page();
					}
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Invalid username or password");
				}
			}

			// If we got this far, something failed, redisplay form
			return Page();
		}
	}
}
