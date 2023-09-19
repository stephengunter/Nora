using Infrastructure.Views;

namespace ApplicationCore.Views;

public class BasePostViewModel : BaseRecordView
{
	public string Title { get; set; } = String.Empty;

	public string? Content { get; set; }
}
