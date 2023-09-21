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
      private readonly IArticlesService _articlesService;
		private readonly IMapper _mapper;

		public ATestsController(IArticlesService articlesService, IMapper mapper)
		{
			_articlesService = articlesService;
			_mapper = mapper;
		}

      [HttpGet("")]
		public async Task<ActionResult> Index()
		{
         string userId = "5a76bf88-783b-42bf-9d8e-7468d456c4da";
			var model = new ArticleViewModel
         {
            Title = "国安部：美国2009年就开始入侵华为总部服务器",
            Content= "美国情报部门凭借其强大的网络攻击武器库，对包括中国在内的全球多国实施监控、窃密和网络攻击，可谓无所不用其极。特别是美国国家安全局，通过其下属的特定入侵行动办公室（TAO）以及先进的武器库，多次对我国进行体系化、平台化攻击，试图窃取我国重要数据资源",
            UserId = userId
         };

         var article = model.MapEntity(_mapper, userId);
			article = await _articlesService.CreateAsync(article);

			return Ok(article.MapViewModel(_mapper));
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
