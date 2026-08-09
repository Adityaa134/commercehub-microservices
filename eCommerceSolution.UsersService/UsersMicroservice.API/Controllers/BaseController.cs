using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UsersService.Core.Exceptions;

namespace UsersMicroservice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected Guid GetUserId()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid parsedUserId))
            {
                throw new UnauthorizedOperationException("You are not authenticated. Please log in to continue.");
            }

            return parsedUserId;
        }
    }
}
