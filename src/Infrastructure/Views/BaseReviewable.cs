namespace Infrastructure.Views;
public abstract class BaseReviewableView : BaseRecordView
{
    public bool Reviewed { get; set; }
}