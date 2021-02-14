using System;
using System.Net;
using Pilchard123.ODSAPI.APIResponses;

namespace Pilchard123.ODSAPI.Models
{
    /// <summary>
    /// The exception that is thrown when the ODS API returns a non-success status
    /// </summary>
    sealed class APIException : Exception
    {
        /// <summary>
        /// The HTTP status code returned from the API
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// The body of the response from the API
        /// </summary>
        public APIError Error { get; }

        /// <summary>
        /// /// Initialises a new instance of the <see cref="APIException"/> class with the specified <see cref="HttpStatusCode"/> and <see cref="ErrorResponse"/>.
        /// </summary>
        /// <param name="statusCode">The HTTP status code from the API</param>
        /// <param name="error">The body of the response from the API</param>
        internal APIException(HttpStatusCode statusCode, APIError error)
        {
            StatusCode = statusCode;
            Error = error;
        }
    }
}
