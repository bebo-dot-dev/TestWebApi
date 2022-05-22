using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using TestWebApi.Models.Response;

namespace TestWebApi.ActionResult
{
    public class JsonBaseResponseResult : JsonResult, IActionResult, IStatusCodeActionResult
    {
        public JsonBaseResponseResult(BaseResponse value) : base(value)
        {
            StatusCode = value.Status;
        }

        public JsonBaseResponseResult(BaseResponse value, object serializerSettings) : base(value, serializerSettings)
        {
            StatusCode = value.Status;
        }
    }
}
