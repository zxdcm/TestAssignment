using Feed.Application.Commands.Comments;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Feed.API.Controllers;

[ApiController]
[Route("api/v1/comments")]
public class CommentController(IMediator mediator)
{
    [HttpPost]
    public async Task<Results<Ok, BadRequest>> CreateComment(CreateCommentCommand createCommentCommand)
    {
        if (createCommentCommand.PostId == Guid.Empty)
        {
            return TypedResults.BadRequest();
        }

        await mediator.Send(createCommentCommand);
        
        return TypedResults.Ok();
    }
    
    [HttpDelete]
    public async Task<Results<Ok, NotFound, BadRequest, ForbidHttpResult>> DeleteComment(DeleteCommentCommand deleteCommentCommand)
    {
        var deleteCommentResult = await mediator.Send(deleteCommentCommand);

        return deleteCommentResult.Match<Results<Ok, NotFound, BadRequest, ForbidHttpResult>>(
            _ => TypedResults.Ok(), 
            error =>
            {
                var firstError = error.First();
                if (firstError == DeleteCommentErrors.NotFound)
                {
                    return TypedResults.NotFound();
                }
                if (firstError == DeleteCommentErrors.Forbidden)
                {
                    return TypedResults.Forbid();
                }

                return TypedResults.BadRequest();
            }
        );
    }
}