using Domain.Errors;
using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Host.Profiles;

public class FluentResultsEndpointProfile : DefaultAspNetCoreResultEndpointProfile
{
    public override ActionResult TransformFailedResultToActionResult(
        FailedResultToActionResultTransformationContext context)
    {
        var error = context.Result.Errors.First();
        return error switch
        {
            ValidationError => new BadRequestObjectResult(error.Message),
            ConflictError => new ConflictObjectResult(error.Message),
            NotFoundError => new NotFoundObjectResult(error.Message),
            _ => new ObjectResult(error.Message)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            }
        };
    }
}