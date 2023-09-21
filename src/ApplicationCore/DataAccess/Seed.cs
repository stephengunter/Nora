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
		string adminPhone = Configuration[$"{SettingsKeys.Admin}:Phone"] ?? "";
		string adminName = Configuration[$"{SettingsKeys.Admin}:Name"] ?? "";

      if(String.IsNullOrEmpty(adminEmail) || String.IsNullOrEmpty(adminPhone))
      {
         throw new Exception("Failed to SeedData. Empty Admin Email/Phone.");
      }
		if(String.IsNullOrEmpty(adminName))
      {
         throw new Exception("Failed to SeedData. Empty Admin Name.");
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
				var user = new User
				{
					Email = adminEmail,			
					UserName = adminEmail,
					Name = adminName,
					PhoneNumber = adminPhone,
					EmailConfirmed = true,
					SecurityStamp = Guid.NewGuid().ToString()
				};
				await CreateUserIfNotExist(userManager, user, new List<string>() { DevRoleName });
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
	
	static async Task CreateUserIfNotExist(UserManager<User> userManager, User newUser, IList<string>? roles = null)
	{
		var user = await userManager.FindByEmailAsync(newUser.Email!);
		if (user == null)
		{
			var result = await userManager.CreateAsync(newUser);

			if (roles!.HasItems())
			{
				await userManager.AddToRolesAsync(newUser, roles!);
			}
		}
		else
		{
			user.PhoneNumber = newUser.PhoneNumber;
			user.Name = newUser.Name;
			await userManager.UpdateAsync(user);
			if (roles!.HasItems())
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
