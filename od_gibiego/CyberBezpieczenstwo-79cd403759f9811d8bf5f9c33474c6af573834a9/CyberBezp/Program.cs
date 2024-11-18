using CyberBezp.Data;
using CyberBezp.Services;
using CyberBezp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CyberBezp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder
				.AddInMemoryDbContext()
				.AddIdentity()
				.AddSeeder()
				.AddRepositories()
				.AddServices();

			builder.Services.AddControllersWithViews();

			builder.Services.ConfigureApplicationCookie(options => { options.LoginPath = "/Identity/Account/Login"; });

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.MapRazorPages();

			using (var scope = app.Services.CreateScope())
			{
				var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
				var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

				roleManager.EnsureRolesCreated().Wait();
				userManager.EnsureAdminCreated().Wait();
			}

			app.Run();
		}
	}

	public static class Extensions
	{
		public static WebApplicationBuilder AddInMemoryDbContext(this WebApplicationBuilder builder)
		{
			builder.Services.AddDbContext<ApplicationDbContext>(options => { options.UseInMemoryDatabase("Trips"); });

			return builder;
		}

		public static WebApplicationBuilder AddPostgresDbContext(this WebApplicationBuilder builder)
		{
			throw new NotImplementedException();

			/*			builder.Services.AddDbContext<ApplicationDbContext>(options =>
						{
							var connection = builder.Configuration.GetConnectionString("postgres") ?? throw new InvalidOperationException("Connection string 'Homework_TripsContextConnection' not found.");

							options.UseNpgsql(connection, b =>
							{
								b.MigrationsAssembly("Homework-Trips");
							});

						});
						return builder;*/
		}

		public static WebApplicationBuilder AddIdentity(this WebApplicationBuilder builder)
		{
			builder.Services.AddDefaultIdentity<IdentityUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;

					options.Password.RequireDigit = true; //Wymagane s¹ cyfry
					options.Password.RequireNonAlphanumeric = true; //Wymagane s¹ znaki niealfanumeryczne (specjalne)
                })
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

			return builder;
		}

		public static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
		{
			return builder;
		}

		public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
		{
			return builder;
		}

		public static WebApplicationBuilder AddAuthorization(this WebApplicationBuilder builder)
		{
			builder.Services.AddAuthorization(options =>
			{
				options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
				options.AddPolicy("User", policy => policy.RequireRole("User"));
			});
			return builder;
		}

		public static WebApplicationBuilder AddSeeder(this WebApplicationBuilder builder)
		{
			builder.Services.AddScoped<ISeeder, Seeder>();
			return builder;
		}

		public static async Task<RoleManager<IdentityRole>> EnsureRolesCreated(this RoleManager<IdentityRole> roleManager)
		{
			string[] roleNames = { "Admin", "User" };
			foreach (var roleName in roleNames)
			{
				var roleExist = await roleManager.RoleExistsAsync(roleName);
				if (!roleExist)
				{
					await roleManager.CreateAsync(new IdentityRole(roleName));
				}
			}

			return roleManager;
		}

		public static async Task<UserManager<IdentityUser>> EnsureAdminCreated(this UserManager<IdentityUser> userManager)
		{
			var adminUser = await userManager.FindByNameAsync("ADMIN");
			if (adminUser is not null) return userManager;

			//Create admin if not present
			adminUser = new IdentityUser
			{
				UserName = "ADMIN",
				Email = "admin@admin.com",
				EmailConfirmed = true
			};

			const string adminPassword = "P@$$w0rd";

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine("Admin created!");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error.Description);
                }
            }
			return userManager;
		}
	}
}