﻿using ApplicationCore.Views;
using ApplicationCore.Models;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ApplicationCore.Authorization;
using ApplicationCore.Helpers;


namespace ApplicationCore.Auth;
public interface IJwtFactory
{
	Task<AccessTokenResponse> GenerateEncodedTokenAsync(User user, IList<string>? roles, OAuth? oAuth = null);
}


public class JwtFactory : IJwtFactory
{
	private readonly IJwtTokenHandler _jwtTokenHandler;
	private readonly JwtIssuerOptions _jwtOptions;

	internal JwtFactory(IJwtTokenHandler jwtTokenHandler, IOptions<JwtIssuerOptions> jwtOptions)
	{
		_jwtTokenHandler = jwtTokenHandler;
		_jwtOptions = jwtOptions.Value;
		ThrowIfInvalidOptions(_jwtOptions);
	}

	public async Task<AccessTokenResponse> GenerateEncodedTokenAsync(User user, IList<string>? roles, OAuth? oAuth = null)
	{
		if (roles.IsNullOrEmpty()) roles = new List<string>();

		var claims = user.CreateClaims(roles!, oAuth);
		claims.Append(new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()));
		claims.Append(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64));
		
		// Create the JWT security token and encode it.
		var jwt = new JwtSecurityToken(
			 _jwtOptions.Issuer,
			 _jwtOptions.Audience,
			 claims,
			 _jwtOptions.NotBefore,
			 _jwtOptions.Expiration,
			 _jwtOptions.SigningCredentials);

		return new AccessTokenResponse(_jwtTokenHandler.WriteToken(jwt), (int)_jwtOptions.ValidFor.TotalSeconds);

	}
	private static long ToUnixEpochDate(DateTime date)
	  => (long)Math.Round((date.ToUniversalTime() -
								  new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
								 .TotalSeconds);

	private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
	{
		if (options == null) throw new ArgumentNullException(nameof(options));

		if (options.ValidFor <= TimeSpan.Zero)
		{
			throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
		}

		if (options.SigningCredentials == null)
		{
			throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
		}

		if (options.JtiGenerator == null)
		{
			throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
		}
	}
}
