using CQRS.Core.Commands;

namespace post.Cmd.Api.Commands
{
    public class AddCommentCommand:BaseCommand
    {
        public string Comment { get; set; }
        public string Username { get; set; }
    }
}
