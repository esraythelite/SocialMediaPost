using CQRS.Core.Commands;

namespace post.Cmd.Api.Commands
{
    public class DeletePostCommand:BaseCommand
    {
        public string Username { get; set; }
    }
}
