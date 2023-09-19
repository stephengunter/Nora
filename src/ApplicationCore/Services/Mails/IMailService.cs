namespace ApplicationCore.Services;

public interface IMailService
{
	Task SendAsync(string email, string subject, string htmlContent, string textContent = "");
}
