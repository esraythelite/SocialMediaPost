using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using post.Cmd.Api.Commands;
using post.Cmd.Api.DTOs;
using Post.Common.DTOs;

namespace post.Cmd.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class NewPostController : ControllerBase
    {
        private readonly ILogger<NewPostController> loggger;
        private readonly ICommandDispatcher commandDispatcher;

        public NewPostController( ILogger<NewPostController> loggger, ICommandDispatcher commandDispatcher )
        {
            this.loggger = loggger;
            this.commandDispatcher = commandDispatcher;
        }

        [HttpPost]
        public async Task<ActionResult> NewPostAsync(NewPostCommand command )
        {
            var id = Guid.NewGuid();
            try
            {
                command.Id = id;

                await commandDispatcher.SendAsync(command);

                return StatusCode(StatusCodes.Status201Created, new NewPostResponse
                {
                    Message = "New post creation request completed successfully"
                });
            }
            catch (InvalidOperationException ex)
            {

                loggger.Log(LogLevel.Warning, ex, "Client made a bad request");
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to create a new post";
                loggger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse
                {
                    Id = id,
                    Message = SAFE_ERROR_MESSAGE
                });
            }

        }
    }
}
