using Lab2.Areas.Identity;
using Lab2.Models;
using Lab2.ViewModels;
using Lab2.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
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
		private readonly UserManager _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IOptions<IdentityOptions> _identityOptions;
		private readonly IOptionsMonitor<CookieAuthenticationOptions> _cookieOptions;


		public AdminController(UserManager userManager, RoleManager<IdentityRole> roleManager, IOptions<IdentityOptions> identityOptions, IOptionsMonitor<CookieAuthenticationOptions> cookieOptions)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_identityOptions = identityOptions;
			_cookieOptions = cookieOptions;
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
				var user = new User { Email = model.Email, EmailConfirmed = true };
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

		[HttpPost]
		public async Task<IActionResult> DeleteUser(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user != null)
			{
				var result = await _userManager.DeleteAsync(user);
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
		public async Task<IActionResult> ChangeUserPasswordChangeInterval(string userId, int days)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user != null)
			{
				user.PasswordValidForDays = days;
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
		public async Task<IActionResult> ChangeUserFunctionParamA(string userId, int paramA)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user != null)
			{
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
	}
}