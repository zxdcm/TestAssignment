using Feed.Application.Commands.Posts;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Feed.API.Controllers;

[ApiController]
[Route("api/v1/posts")]
public class PostController(IMediator mediator)
{
    [HttpPost]
    public async Task<Results<Ok, BadRequest>> CreatePostCommand(CreatePostCommand createPostCommand)
    {
        _ = await mediator.Send(createPostCommand);
        
        return TypedResults.Ok();
    }
}