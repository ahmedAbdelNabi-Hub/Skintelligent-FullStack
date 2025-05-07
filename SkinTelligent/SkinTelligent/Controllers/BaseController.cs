using Microsoft.AspNetCore.Mvc;
using SkinTelIigent.Core.Interface;
using SkinTelIigentContracts.CustomResponses;
using System.Net;

namespace SkinTelligent.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {

        protected async Task<ActionResult<BaseApiResponse>> SaveChangesAsync(IUnitOfWork unitOfWork, string successMessage, string failureMessage)
        {
            try
            {
                var result = await unitOfWork.SaveChangeAsync();
                if (result > 0)
                {
                    return Ok(new BaseApiResponse((int)HttpStatusCode.OK, successMessage));
                }
                return BadRequest(new BaseApiResponse((int)HttpStatusCode.BadRequest, failureMessage));
            }
            catch (Exception ex)
            {
                return new BaseApiResponse(StatusCodes.Status500InternalServerError, ex.InnerException?.Message);
            }
        }


        protected ActionResult<BaseApiResponse> HandleStatusCode(BaseApiResponse response)
        {
            return response.statusCode switch
            {
                StatusCodes.Status200OK => Ok(response),
                StatusCodes.Status201Created => Created(string.Empty, response),
                StatusCodes.Status400BadRequest => BadRequest(response),
                StatusCodes.Status401Unauthorized => Unauthorized(response),
                StatusCodes.Status403Forbidden => Forbid(response.message),
                StatusCodes.Status404NotFound => NotFound(response),
                StatusCodes.Status500InternalServerError => StatusCode(500, response),
                _ => StatusCode(response.statusCode, response)
            };
        }
    }
}
