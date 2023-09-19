using System;
using Mailjet.Client;
using Mailjet.Client.Resources;
using ApplicationCore.Exceptions;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using ApplicationCore.Helpers;
using System.Net;
using Mailjet.Client.TransactionalEmails;
using Newtonsoft.Json.Linq;
using PostmarkDotNet;
using PostmarkDotNet.Model;

namespace ApplicationCore.Services;

public class PostMarkService : IMailService
{
   private readonly string _apiKeyPublic;
   private readonly string _apiKeySecret;
	private readonly AppSettings _appSettings;
	private readonly MailSettings _mailSettings;

	public PostMarkService(IOptions<AppSettings> appSettings, IOptions<MailSettings> mailSettings)
	{
		_appSettings = appSettings.Value;
      _mailSettings = mailSettings.Value;
      _apiKeyPublic = _mailSettings.Key;
      _apiKeySecret = _mailSettings.Secret;
	}

   public async Task SendAsync(string email, string subject, string htmlContent, string textContent = "")
   {
      var message = new PostmarkMessage()
      {
         To = "traders.com.tw@gmail.com",
         From = "service@exam-learner.com",
         TrackOpens = true,
         Subject = "A complex email",
         TextBody = "Plain Text Body",
         HtmlBody = "<html><body><h1>Test</h1></body></html>",
         Tag = "business-message"
      };

      var client = new PostmarkClient("1c831605-10b2-4064-9fae-fcc504e4e4b9");
      var sendResult = await client.SendMessageAsync(message);

      

      Console.WriteLine($"Msg: {sendResult.Message}");
      Console.WriteLine($"Status: {sendResult.Status.ToString()}");
      Console.WriteLine($"ErrorCode: {sendResult.ErrorCode}");
   }
   
}