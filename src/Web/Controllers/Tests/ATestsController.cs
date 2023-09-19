using ApplicationCore.Helpers;
using ApplicationCore.Services;
using ApplicationCore.Settings;
using ApplicationCore.Views;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using ApplicationCore.DataAccess;
using System.IO;
using ApplicationCore.Models;
using Newtonsoft.Json;
using Web.Models;

namespace Web.Controllers.Tests;

public class ATestsController : BaseTestController
{
   private readonly MailSettings _mailSettings;
   private readonly IMailService _mailService;
   private readonly IMapper _mapper;
   public ATestsController(IOptions<MailSettings> mailSettings, IMailService mailService, IMapper mapper)
   {
      _mailSettings = mailSettings.Value;
      _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
      _mapper = mapper;
   }

   [HttpGet]
   public async Task<ActionResult>  Index()
   {
      
      await _mailService.SendAsync("traders.com.tw@gmail.com", "Stest", "Ctest");
		return Ok();
   }


   [HttpGet("version")]
   public ActionResult Version()
   {
      return Ok();
      //return Ok(_appSettings.ApiVersion);
   }


   [HttpGet("ex")]
   public ActionResult Ex()
   {
      throw new System.Exception("Test Throw Exception");
   }
}
