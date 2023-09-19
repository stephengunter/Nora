using Ardalis.Specification;
using ApplicationCore.Models;

namespace ApplicationCore.Specifications;
public class ArticleSpecification : Specification<Article>
{
	public ArticleSpecification()
	{
		Query.Where(item => !item.Removed);
	}
	public ArticleSpecification(int categoryId)
	{
		Query.Where(item => !item.Removed && item.CategoryId == categoryId);
	}

}

