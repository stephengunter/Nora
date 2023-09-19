using Infrastructure.Views;
using Microsoft.AspNetCore.Identity;

namespace ApplicationCore.Helpers;

public static class RolesHelpers
{	
   public static BaseOption<string> ToOption(this IdentityRole role) => new BaseOption<string>(role.Name!, role.Name!);

}
