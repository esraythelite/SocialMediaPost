using Post.Common.DTOs;

namespace post.Cmd.Api.DTOs
{
    public class NewPostResponse:BaseResponse
    {
        public Guid Id { get; set; }
    }
}
