using System.Net;

namespace UmbracoBridge.Domain.Exceptions
{
    public class ExternalServiceException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public ExternalServiceException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public ExternalServiceException(string message, HttpStatusCode statusCode, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
