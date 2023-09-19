using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ApplicationCore.Models;
using ApplicationCore.Helpers;
using ApplicationCore.Settings;
using Microsoft.Extensions.Configuration;

namespace ApplicationCore.DataAccess;

public static class SeedData
{
	static string SubscriberRoleName = AppRoles.Subscriber.ToString();
	static string DevRoleName = AppRoles.Dev.ToString();
	static string BossRoleName = AppRoles.Boss.ToString();
	public static async Task EnsureSeedData(IServiceProvider serviceProvider, ConfigurationManager Configuration)
   {
		string adminEmail = Configuration[$"{SettingsKeys.Admin}:Email"] ?? "";
      if(String.IsNullOrEmpty(adminEmail))
      {
         throw new Exception("Failed to SeedData. Empty AdminEmail.");
      }
		
		Console.WriteLine("Seeding database...");
		var context = serviceProvider.GetRequiredService<DefaultContext>();
	   context.Database.EnsureCreated();
		
		using (var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>())
		{
			await SeedRoles(roleManager);
		}
		if(!String.IsNullOrEmpty(adminEmail)) {
			using (var userManager = serviceProvider.GetRequiredService<UserManager<User>>())
			{
				await CreateUserIfNotExist(userManager, adminEmail, new List<string>() { DevRoleName });
			}
		}
		

      Console.WriteLine("Done seeding database.");
	}

	static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
	{
		var roles = new List<string> { DevRoleName, BossRoleName, SubscriberRoleName };
		foreach (var item in roles) await AddRoleIfNotExist(roleManager, item);
	}
	static async Task AddRoleIfNotExist(RoleManager<IdentityRole> roleManager, string roleName)
	{
		var role = await roleManager.FindByNameAsync(roleName);
		if (role == null) await roleManager.CreateAsync(new IdentityRole { Name = roleName });
	}
	
	static async Task CreateUserIfNotExist(UserManager<User> userManager, string email, IList<string>? roles = null)
	{
		var user = await userManager.FindByEmailAsync(email);
		if (user == null)
		{
			bool isAdmin = false;
			if (roles!.HasItems())
			{
				isAdmin = roles!.Select(r => r.EqualTo(DevRoleName) || r.EqualTo(BossRoleName)).FirstOrDefault();
			}

			var newUser = new User
			{
				Email = email,			
				UserName = email,
				EmailConfirmed = isAdmin,
				SecurityStamp = Guid.NewGuid().ToString()
			};


			var result = await userManager.CreateAsync(newUser);

			if (!roles.IsNullOrEmpty())
			{
				await userManager.AddToRolesAsync(newUser, roles!);
			}


		}
		else
		{
			if (!roles.IsNullOrEmpty())
			{
				foreach (var role in roles!)
				{
					bool hasRole = await userManager.IsInRoleAsync(user, role);
					if (!hasRole) await userManager.AddToRoleAsync(user, role);
				}
			}

		}
	}
}
