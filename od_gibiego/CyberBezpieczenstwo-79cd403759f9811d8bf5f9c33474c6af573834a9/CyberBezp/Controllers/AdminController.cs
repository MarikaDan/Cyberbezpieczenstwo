using CyberBezp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CyberBezp.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminController : Controller
	{
		//extract to a service later
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IOptions<IdentityOptions> _identityOptions;

		public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<IdentityOptions> identityOptions)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_identityOptions = identityOptions;

		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult AddUser()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> LockUser(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user != null)
			{
				user.LockoutEnd = DateTimeOffset.MaxValue; // Lock the user indefinitely
				var result = await _userManager.UpdateAsync(user);
				if (result.Succeeded)
				{
					return RedirectToAction("Index");
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}
			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<IActionResult> UnlockUser(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user != null)
			{
				user.LockoutEnd = null; // Unlock the user
				var result = await _userManager.UpdateAsync(user);
				if (result.Succeeded)
				{
					return RedirectToAction("Index");
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}
			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<IActionResult> AddUser(AddUserViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new IdentityUser { UserName = model.UserName, Email = model.Email, EmailConfirmed = true };
				var result = await _userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					if (!await _roleManager.RoleExistsAsync(model.Role))
					{
						await _roleManager.CreateAsync(new IdentityRole(model.Role));
					}
					await _userManager.AddToRoleAsync(user, model.Role);
					return RedirectToAction("Index", "Home");
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}
			return View(model);
		}

		[HttpGet]
		public IActionResult ConfigurePassword()
		{
			var model = new PasswordRequirementsViewModel
			{
				RequireDigit = _identityOptions.Value.Password.RequireDigit,
				RequireLowercase = _identityOptions.Value.Password.RequireLowercase,
				RequireNonAlphanumeric = _identityOptions.Value.Password.RequireNonAlphanumeric,
				RequireUppercase = _identityOptions.Value.Password.RequireUppercase,
				RequiredLength = _identityOptions.Value.Password.RequiredLength,
				RequiredUniqueChars = _identityOptions.Value.Password.RequiredUniqueChars
			};
			return View(model);
		}

		[HttpPost]
		public IActionResult ConfigurePassword(PasswordRequirementsViewModel model)
		{
			if (ModelState.IsValid)
			{
				_identityOptions.Value.Password.RequireDigit = model.RequireDigit;
				_identityOptions.Value.Password.RequireLowercase = model.RequireLowercase;
				_identityOptions.Value.Password.RequireNonAlphanumeric = model.RequireNonAlphanumeric;
				_identityOptions.Value.Password.RequireUppercase = model.RequireUppercase;
				_identityOptions.Value.Password.RequiredLength = model.RequiredLength;
				_identityOptions.Value.Password.RequiredUniqueChars = model.RequiredUniqueChars;

				// Save the changes to the configuration (if needed)
				// This might involve updating a configuration file or database

				return RedirectToAction("Index", "Home");
			}
			return View(model);
		}
	}
}