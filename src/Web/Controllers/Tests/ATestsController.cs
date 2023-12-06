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
   private readonly AdminSettings _adminSettings;
   private readonly AppSettings _appSettings;
   private readonly IArticlesService _articlesService;
   private readonly IMapper _mapper;

   public ATestsController(IOptions<AdminSettings> adminSettings, IOptions<AppSettings> appSettings,
      IArticlesService articlesService, IMapper mapper)
   {
      _adminSettings = adminSettings.Value;
      _appSettings = appSettings.Value;
      _articlesService = articlesService;
      _mapper = mapper;
   }

   [HttpGet("")]
   public async Task<ActionResult>  Index(string key)
   {
      if (String.IsNullOrEmpty(key) || key != _adminSettings.Key) ModelState.AddModelError("key", "認證錯誤");
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var articles = await _articlesService.FetchAllAsync();		
		if (articles.HasItems())
		{
			articles = articles.GetOrdered().ToList();
		}
		return Ok(articles.MapViewModelList(_mapper));
   }
   [HttpPost("")]
	public async Task<ActionResult> Store([FromBody] AdminRequest model)
	{
		if (model.Key != _adminSettings.Key) ModelState.AddModelError("key", "認證錯誤");
      if (String.IsNullOrEmpty(model.Data)) ModelState.AddModelError("data", "資料錯誤");
      if (!ModelState.IsValid) return BadRequest(ModelState);

		var article = new Article
      {
         Title = model.Data,
         Content = "test Content",
         UserId = _adminSettings.Id
      };

		article = await _articlesService.CreateAsync(article);

		return Ok(article.MapViewModel(_mapper));
	}

   [HttpGet("version")]
   public ActionResult Version()
   {
      return Ok(_appSettings.ApiVersion);
   }


   [HttpGet("ex")]
   public ActionResult Ex()
   {
      throw new System.Exception("Test Throw Exception");
   }
}
