using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;

namespace ApplicationCore.Services;
public interface IArticlesService
{
	Task<IEnumerable<Article>> FetchAsync(Category category);
	Task<Article?> GetByIdAsync(int id);
	Task<Article> CreateAsync(Article article);

	Task<IEnumerable<Article>> FetchAllAsync();
	Task UpdateAsync(Article article);
	void Update(Article existingEntity, Article model);
	Task RemoveAsync(Article article);


	#region  Categories
	Task<Category> CreateAsync(Category category);
	Task<IEnumerable<Category>> FetchCategoriesAsync();

	Task<Category?> FindCategoryByKeyAsync(string key);

	#endregion
}

public class ArticlesService : IArticlesService
{
	private readonly IDefaultRepository<Article> _articleRepository;
	private readonly IDefaultRepository<Category> _categoryRepository;
	public ArticlesService(IDefaultRepository<Article> articleRepository, IDefaultRepository<Category> categoryRepository)
	{
		_articleRepository = articleRepository;
		_categoryRepository = categoryRepository;
	}

	
	public async Task<IEnumerable<Article>> FetchAsync(Category category)
		=> await _articleRepository.ListAsync(new ArticleSpecification(category));

	public async Task<IEnumerable<Article>> FetchAllAsync()
		=> await _articleRepository.ListAsync(new ArticleSpecification());

	public async Task<Article?> GetByIdAsync(int id) => await _articleRepository.GetByIdAsync(id);
	
	public async Task<Article> CreateAsync(Article article) => await _articleRepository.AddAsync(article);


	public async Task UpdateAsync(Article article) => await _articleRepository.UpdateAsync(article);
	public void Update(Article existingEntity, Article model) => _articleRepository.Update(existingEntity, model);
	public async Task RemoveAsync(Article article)
	{
		
		article.Removed = true;
		await _articleRepository.UpdateAsync(article);
	}

	#region  Categories
	public async Task<Category> CreateAsync(Category category)
		=> await _categoryRepository.AddAsync(category);
	public async Task<IEnumerable<Category>> FetchCategoriesAsync() 
		=> await _categoryRepository.ListAsync(new CategoriesSpecification());

	public async Task<Category?> FindCategoryByKeyAsync(string key)
		=> await _categoryRepository.FirstOrDefaultAsync(new CategoriesSpecification(key));

	#endregion
}
