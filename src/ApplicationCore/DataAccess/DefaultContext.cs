using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;

namespace ApplicationCore.DataAccess;
public class DefaultContext : IdentityDbContext<User>
{
	public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
	{
	}
	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		foreach (var property in builder.Model.GetEntityTypes()
			.SelectMany(t => t.GetProperties())
			.Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?))
      )
		{
			property.SetColumnType("timestamp without time zone");
		}
	}

	#region Auth
	public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
	public DbSet<OAuth> OAuth => Set<OAuth>();
	#endregion

	#region Articles	
	public DbSet<Article> Articles => Set<Article>();
	public DbSet<UploadFile> UploadFiles => Set<UploadFile>();
	public DbSet<Category> Categories => Set<Category>();
	#endregion

	public override int SaveChanges() => SaveChangesAsync().GetAwaiter().GetResult();

}
