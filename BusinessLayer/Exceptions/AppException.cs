

namespace BusinessLayer.Exceptions
{
    public class AppException : Exception
    {
        public int StatusCode { get; set; }
        public List<string> Errors { get; set; } // Validation hatalarını saklayacak liste

        public AppException(int statusCode, string message, List<string> errors = null) : base(message)
        {
            this.StatusCode = statusCode;
            this.Errors = errors ?? new List<string>(); // Boş bir liste oluşturun
        }
    }
}
