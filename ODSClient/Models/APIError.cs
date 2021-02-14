namespace Pilchard123.ODSAPI.Models
{
    /// <summary>
    /// Represents an error returned from the API.
    /// </summary>
    class APIError
    {
        /// <summary>
        /// The error code reurned from the API. Usually the same as the HTTP status code, but the API documentation does not actually specify that to be the case.
        /// </summary>
        public int ErrorCode { get; set; }
        /// <summary>
        /// A short description of the problem with the request. More specific than a standard HTTP status code.
        /// </summary>
        public string ErrorText { get; set; }

        internal APIError(int errorCode, string errorText)
        {
            ErrorCode = errorCode;
            ErrorText = errorText;
        }
    }
}
