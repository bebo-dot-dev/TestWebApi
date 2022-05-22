using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TestWebApi.Models.Response
{
    public class BaseResponse : ProblemDetails
    {
        public BaseResponse()
        {
            Type = GetType().Name;
        }

        public IList<ErrorDetail> Errors { get; set; }
    }

    public class ErrorDetail
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
