using CQRS.Core.Commands;

namespace post.Cmd.Api.Commands
{
    public class EditMessageCommand:BaseCommand
    {
        public string Message { get; set; }
    }
}
