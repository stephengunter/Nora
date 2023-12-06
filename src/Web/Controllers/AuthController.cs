using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Models;
using ApplicationCore.Auth;
using ApplicationCore.Services;
using Microsoft.Extensions.Options;
using ApplicationCore.Settings;
using ApplicationCore.Authorization;
using ApplicationCore.Helpers;

namespace Web.Controllers;

public class AuthController : BaseController
{
	private readonly IUsersService _usersService;
	private readonly IAuthService _authService;
	private readonly AdminSettings _adminSettings;


	public AuthController(IOptions<AdminSettings> adminSettings,IUsersService usersService, IAuthService authService)
	{
		_adminSettings = adminSettings.Value;
		_usersService = usersService;
		_authService = authService;
	}

	[HttpPost("")]
	public async Task<ActionResult> Login([FromBody] OAuthLoginRequest model)
	{
		var user = _usersService.FindUserByPhone(model.Token);

		if (user == null)
		{
			ModelState.AddModelError("auth", "登入失敗.");
			return BadRequest(ModelState);
		}

		var roles = await _usersService.GetRolesAsync(user);

		var responseView = await _authService.CreateTokenAsync(RemoteIpAddress, user, roles);

		return Ok(responseView);;
	}

	//POST api/auth/refreshtoken
	[HttpPost("refreshtoken")]
	public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest model)
	{
		var cp = _authService.ResolveClaimsFromToken(model.AccessToken);
		if(cp is null) 
		{
			ModelState.AddModelError("token", "身分驗證失敗. 請重新登入");
			return BadRequest(ModelState);
		}
		string userId = cp!.GetUserId();
		var oauthProvider = cp.GetOAuthProvider();
		var user = await _usersService.FindByIdAsync(userId);
		if(user is null)
		{
		   throw new Exception($"RefreshToken Failed. User NotFound By Id: {userId}");
		}
		await ValidateRequestAsync(model, user);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		var roles = await _usersService.GetRolesAsync(user);
		var oauth = await _authService.FindOAuthByProviderAsync(user!, oauthProvider);
		//if(oauth is null)  throw new Exception($"RefreshToken Failed. OAuth NotFound By Provider: {oauthProvider.ToString()}");
		
		var responseView = await _authService.CreateTokenAsync(RemoteIpAddress, user, roles, oauth);
		return Ok(responseView);

	}

	async Task ValidateRequestAsync(RefreshTokenRequest model, User user)
	{
		bool isValid = await _authService.IsValidRefreshTokenAsync(model.RefreshToken, user);
		if(!isValid) ModelState.AddModelError("token", "身分驗證失敗. 請重新登入");
	}



}

