using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using ApplicationCore.Auth;
using ApplicationCore.Helpers;

namespace ApplicationCore.DI;

public static class JwtBearerDI
{
	public static void AddJwtBearer(this IServiceCollection services, ConfigurationManager Configuration)
	{
		string securityKey = Configuration[$"{SettingsKeys.Auth}:SecurityKey"] ?? "";
		if(String.IsNullOrEmpty(securityKey))
		{
			throw new Exception("Failed Add AddJwtBearer. Empty SecurityKey.");
		}

		string issuer = Configuration[$"{SettingsKeys.App}:Name"] ?? "";
		if(String.IsNullOrEmpty(issuer))
		{
			throw new Exception("Failed Add AddJwtBearer. Empty AppName.");
		}

		string audience = Configuration[$"{SettingsKeys.App}:ClientUrl"] ?? "";
		if(String.IsNullOrEmpty(audience))
		{
			throw new Exception("Failed Add AddJwtBearer. Empty ClientUrl.");
		}

		string tokenValidHours = Configuration[$"{SettingsKeys.Auth}:TokenValidHours"] ?? "";		
		int tokenValidHoursValue = tokenValidHours.ToInt();
		if(tokenValidHoursValue < 1)
		{
			throw new Exception("Failed Add AddJwtBearer. Invalid TokenValidHours.");
		}

		AddJwtBearer(services, tokenValidHoursValue, issuer, audience, securityKey);

	}
	static void AddJwtBearer(IServiceCollection services, int tokenValidHours, string issuer, string audience, string securityKey)
	{
		var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(securityKey));
		services.Configure<JwtIssuerOptions>(options =>
		{
			options.ValidFor = TimeSpan.FromHours(tokenValidHours);
			options.Issuer = issuer;
			options.Audience = audience;
			options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
		});

		JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
		var tokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidIssuer = issuer,

			ValidateAudience = true,
			ValidAudience = audience,

			ValidateIssuerSigningKey = true,
			IssuerSigningKey = signingKey,

			RequireExpirationTime = false,
			ValidateLifetime = true,
			ClockSkew = TimeSpan.Zero
		};

		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

		})
		.AddJwtBearer(configureOptions =>
		{
			configureOptions.ClaimsIssuer = issuer;
			configureOptions.TokenValidationParameters = tokenValidationParameters;
			configureOptions.SaveToken = true;

			configureOptions.Events = new JwtBearerEvents
			{
				OnAuthenticationFailed = context =>
				{
					if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
					{
						context.Response.Headers.Add("Token-Expired", "true");
					}
					return Task.CompletedTask;
				}
			};
		});
	}
}
