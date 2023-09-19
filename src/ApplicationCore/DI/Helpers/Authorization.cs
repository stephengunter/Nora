using ApplicationCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationCore.DI;
public static class AuthorizationDI
{
   public static void AddAuthorizationPolicy(this IServiceCollection services)
   {
      services.AddAuthorization(options =>
      {
         options.AddPolicy(Permissions.Subscriber.ToString(), policy =>
            policy.Requirements.Add(new HasPermissionRequirement(Permissions.Subscriber)));

         options.AddPolicy(Permissions.Admin.ToString(), policy =>
            policy.Requirements.Add(new HasPermissionRequirement(Permissions.Admin)));
      });
	}
}
