namespace ApplicationCore;

public class SettingsKeys
{
	public static string App = "App";
	public static string Auth = "Auth";
	public static string Admin = "Admin";
	public static string Mail = "Mail";
	public static string EcPay = "EcPay";
	public static string Subscribes = "Subscribes";
	public static string CloudStorage = "CloudStorage";
}
public enum AppRoles
{
	Boss,
	Dev,
	Subscriber
}
public enum Permissions
{
	Admin,
	Subscriber

}
public static class JwtClaimIdentifiers
{
	public const string Rol = "rol";
	public const string Id = "id";
	public const string Sub = "sub";
	public const string Roles = "roles";
	public const string Provider = "provider";
	public const string Picture = "picture";
	public const string Name = "name";
}

public static class JwtClaims
{
	public const string ApiAccess = "api_access";
}

public enum PaymentTypes
{
	CREDIT,
	ATM
}

public enum ThirdPartyPayment
{
	EcPay
}
public enum HttpClients
{
	Google
}
