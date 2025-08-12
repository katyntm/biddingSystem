namespace CarAuction.Application.Common.Exceptions
{
    public class ValidationException : Exception
    {
        public int StatusCode { get; set; }
        public ValidationException(string message, int statusCode = 400) : base(message) 
        {
            StatusCode = statusCode;
        }
    }
}
