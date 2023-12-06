using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Services;
using ApplicationCore.Models;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using AutoMapper;
using Web.Models;

namespace Web.Controllers.Admin;

public class ArticlesController : BaseAdminController
{
	private readonly IArticlesService _articlesService;
	private readonly IMapper _mapper;

	public ArticlesController(IArticlesService articlesService, IMapper mapper)
	{
		_articlesService = articlesService;
		_mapper = mapper;
	}

	[HttpGet("")]
	public async Task<ActionResult> Index(int category, int active, int page = 1, int pageSize = 10)
	{
		IEnumerable<Article> articles;
		if(category > 0) articles = await _articlesService.FetchAsync(new Category{ Id = category});
		else  articles = await _articlesService.FetchAllAsync();
		
		if (articles.HasItems())
		{
			articles = articles.Where(x => x.Active == active.ToBoolean());

			articles = articles.GetOrdered().ToList();
		}
		return Ok(articles.GetPagedList(_mapper, page, pageSize));
	}


	[HttpGet("create")]
	public ActionResult Create()
	{
		return Ok(new ArticleViewModel());
	}
	[HttpPost("")]
	public async Task<ActionResult> Store([FromBody] ArticleViewModel model)
	{
		ValidateRequest(model);
		if(!ModelState.IsValid) return BadRequest(ModelState);

		var article = model.MapEntity(_mapper, CurrentUserId);
		article.Order = model.Active ? 0 : - 1;

		article = await _articlesService.CreateAsync(article);

		return Ok(article.MapViewModel(_mapper));
	}

	[HttpGet("edit/{id}")]
	public async Task<ActionResult> Edit(int id)
	{
		var article = await _articlesService.GetByIdAsync(id);
		if (article == null) return NotFound();

		var model = article.MapViewModel(_mapper);

		return Ok(model);
	}

	[HttpPut("{id}")]
	public async Task<ActionResult> Update(int id, [FromBody] ArticleViewModel model)
	{
		var article = await _articlesService.GetByIdAsync(id);
		if (article == null) return NotFound();

		ValidateRequest(model);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		article = model.MapEntity(_mapper, CurrentUserId, article);

		await _articlesService.UpdateAsync(article);

		return Ok();
	}

	void ValidateRequest(ArticleViewModel model)
	{
		if(String.IsNullOrEmpty(model.Title)) ModelState.AddModelError("title", "必須填寫標題");

		if(String.IsNullOrEmpty(model.Content)) ModelState.AddModelError("content", "必須填寫內容");

	}

}

