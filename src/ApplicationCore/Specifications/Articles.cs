using Ardalis.Specification;
using ApplicationCore.Models;

namespace ApplicationCore.Specifications;
public class ArticleSpecification : Specification<Article>
{
	public ArticleSpecification()
	{
		Query.Where(item => !item.Removed);
	}
	public ArticleSpecification(Category category)
	{
		Query.Where(item => !item.Removed && item.CategoryId == category.Id);
	}

}

