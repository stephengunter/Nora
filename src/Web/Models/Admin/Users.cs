using ApplicationCore.Models;
using ApplicationCore.Helpers;
using ApplicationCore.Paging;
using ApplicationCore.Views;
using Infrastructure.Views;
using Microsoft.AspNetCore.Identity;

namespace Web.Models;

public class UsersAdminModel
{
    public ICollection<BaseOption<string>> RolesOptions { get; set; } = new List<BaseOption<string>>();

    public PagedList<User, UserViewModel>? PagedList { get; set; }
    public void LoadRolesOptions(IEnumerable<IdentityRole> roles, string emptyText = "全部")
    {
        var options = roles.Select(x => x.ToOption()).ToList();

        if (!String.IsNullOrEmpty(emptyText)) options.Insert(0, new BaseOption<string>("", emptyText));

        RolesOptions = options;
    }
}
