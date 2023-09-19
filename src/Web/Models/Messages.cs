using ApplicationCore.Views;

namespace Web.Models
{
   public class MessageEditForm : AnonymousRequest
   {
      public MessageEditForm(string token) : base(token)
      {

      }
      public MessageViewModel Message  { get; set; } = new MessageViewModel();
   }
}
