using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using TestWebApi.ActionResult;
using TestWebApi.Models.Request;
using TestWebApi.Models.Response;

namespace TestWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        [Route("validate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(BaseResponse), 400)]
        [ProducesResponseType(typeof(BaseResponse), 500)]
        public ActionResult<ResponseModel1> GetAction([FromQuery] RequestModel request)
        {
            return new JsonBaseResponseResult(
                new ResponseModel1
                {
                    Id = request.Id.Value,
                    Name = request.Name,
                    Rating = request.Rating.Value
                });
        }

        [HttpPost]
        [Route("validate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(BaseResponse), 400)]
        [ProducesResponseType(typeof(BaseResponse), 500)]
        public ActionResult<ResponseModel2> PostAction([FromBody] RequestModel request)
        {
            return new JsonBaseResponseResult(
                new ResponseModel2
                {
                    Descriptor = request.Name
                });
        }

        [HttpGet]
        [Route("throwException")]
        [ProducesResponseType(typeof(BaseResponse), 500)]
        public IActionResult ExceptionAction()
        {
            throw new Exception("ExceptionAction");
        }

        [HttpPost]
        [Route("returnErrorDetails")]
        [ProducesResponseType(409)]
        public ActionResult<ResponseModel1> PostActionReturnErrorDetails()
        {
            return new JsonBaseResponseResult(
                new ResponseModel1
                {
                    Id = 123,
                    Name = "error response",
                    Status = (int)HttpStatusCode.Conflict,
                    Errors = new List<ErrorDetail>
                    {
                        new ErrorDetail { Name = "error1", Description = "error1Description" }
                    }
                });
        }
    }
}
