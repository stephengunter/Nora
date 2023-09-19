using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using ApplicationCore;
using ApplicationCore.Models;
using ApplicationCore.Settings;
using ApplicationCore.DI;
using ApplicationCore.DataAccess;
using Microsoft.AspNetCore.Identity;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web application");
    
    var builder = WebApplication.CreateBuilder(args);
    var Configuration = builder.Configuration;

    builder.Host.UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext());

    #region Autofac
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
    builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule<ApplicationCoreModule>();
    });
    #endregion

    #region Add Configurations
    builder.Services.Configure<AppSettings>(Configuration.GetSection(SettingsKeys.App));
    builder.Services.Configure<AdminSettings>(Configuration.GetSection(SettingsKeys.Admin));
    builder.Services.Configure<AuthSettings>(Configuration.GetSection(SettingsKeys.Auth));
    builder.Services.Configure<MailSettings>(Configuration.GetSection(SettingsKeys.Mail));
    #endregion
    

    // Add services to the container.
    string connectionString = Configuration.GetConnectionString("Default")!;
    builder.Services.AddDbContext<DefaultContext>(options =>
                    options.UseNpgsql(connectionString));

    #region AddIdentity
    builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
    }).AddEntityFrameworkStores<DefaultContext>()
        .AddDefaultTokenProviders();
    #endregion

    #region AddJwtBearer
    builder.Services.AddJwtBearer(Configuration);
    #endregion
    
    #region AddAuthorization
    builder.Services.AddAuthorizationPolicy();
    #endregion

    #region Add Cors
    builder.Services.AddCorsPolicy(Configuration);
    #endregion

    builder.Services.AddDtoMapper();


    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwagger(Configuration);

    var app = builder.Build();
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {        
        // Seed Database
	    using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
	        {
	            await SeedData.EnsureSeedData(services, Configuration);
	        }
            catch (Exception ex)
            {
                Log.Fatal(ex, "SeedData Error");
            }
        }


        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseHttpsRedirection();
    }

    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.Information("finally");
    Log.CloseAndFlush();
}

