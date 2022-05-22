using System;

namespace TestWebApi.Validation
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message = null)
            : base(message ?? "Bad Request")
        { }
    }
}
