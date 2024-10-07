using App.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace App.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomBaseController : ControllerBase
    {
        [NonAction]
        public IActionResult CreateActionResult<T>(ServiceResult<T> result)
        {
            //switch expression
            return result.StatusCode switch
            {
                HttpStatusCode.NoContent => NoContent(),
                HttpStatusCode.Created => Created(result.UrlAsCreated, result),
                _ => new ObjectResult(result) { StatusCode = result.StatusCode.GetHashCode() }
            };
        }

        [NonAction]
        public IActionResult CreateActionResult(ServiceResult result)
        {
            //switch expression
            return result.StatusCode switch
            {
                HttpStatusCode.NoContent => new ObjectResult(null) { StatusCode = result.StatusCode.GetHashCode() },
                _ => new ObjectResult(result) { StatusCode = result.StatusCode.GetHashCode()}
            };
        }
    }
}
