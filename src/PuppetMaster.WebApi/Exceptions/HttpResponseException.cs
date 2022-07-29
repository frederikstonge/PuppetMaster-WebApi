using System.Net;

namespace System
{
    public class HttpResponseException : Exception
    {
        public HttpResponseException(HttpStatusCode statusCode = HttpStatusCode.InternalServerError, object? value = null) => 
            (StatusCode, Value) = (statusCode, value);

        public HttpStatusCode StatusCode { get; }

        public object? Value { get; }
    }
}
