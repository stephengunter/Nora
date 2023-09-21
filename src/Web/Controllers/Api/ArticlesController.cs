
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers.Api
{
	// [Authorize]
	public class ArticlesController : BaseApiController
	{
		private readonly IArticlesService _articlesService;
		private readonly IUsersService _usersService;
		private readonly IMapper _mapper;

		public ArticlesController(IArticlesService articlesService, IUsersService usersService, IMapper mapper)
		{
			_articlesService = articlesService;
			_usersService = usersService;
			_mapper = mapper;
		}
	

		[HttpGet("")]
		public async Task<ActionResult> Index(int page = 1, int pageSize = 99)
		{
			if (page < 1) page = 1;

			var articles = await _articlesService.FetchAllAsync();

			articles = articles.Where(x => x.Active);

			articles = articles.GetOrdered().ToList();

			return Ok(articles.GetPagedList(_mapper, page, pageSize));
		}


		[HttpGet("{id}")]
		public async Task<ActionResult> Details(int id)
		{
			var article = await _articlesService.GetByIdAsync(id);
			if (article == null) return NotFound();

			if (!article.Active) return NotFound();

			return Ok(article.MapViewModel(_mapper));
		}



	}

	
}
