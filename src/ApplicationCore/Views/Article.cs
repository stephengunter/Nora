using Infrastructure.Views;

namespace ApplicationCore.Views;

public class ArticleViewModel : BasePostViewModel
{
	public int? CategoryId { get; set; }
	public string? Summary { get; set; } = String.Empty;
	public string UserId { get; set; } = String.Empty;

	public virtual ICollection<AttachmentViewModel> Attachments { get; set; } = new List<AttachmentViewModel>();

}

