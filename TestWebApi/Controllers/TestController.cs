using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using TestWebApi.Models;

namespace TestWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {        
        [HttpPost]
        [Route("validate")]
        public IActionResult PostAction([FromBody] RequestModel request)
        {
            return new JsonResult(
                new ResponseModel
                {
                    Id = request.Id.Value,
                    Name = request.Name,
                    Rating = request.Rating
                });
        }

        [HttpPost]
        [Route("returnErrorDetails")]
        public IActionResult PostActionReturnsErrorDetails()
        {
            return new JsonResult(
                new ResponseModel
                {
                    Id = 123,
                    Name = "error response",
                    Status = (int)HttpStatusCode.Conflict,
                    Errors = new List<ErrorDetail>
                    {
                        new ErrorDetail { Name = "error1", Description = "error1Description" }
                    }
                })
            {
                StatusCode = (int)HttpStatusCode.Conflict
            };
        }
    }
}
