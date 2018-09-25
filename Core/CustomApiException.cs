using System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core
{
    public class CustomApiException : Exception
    {
        private readonly ProblemDetails _problemDetails;

        public ProblemDetails ProblemDetails
        {
            get
            {
                _problemDetails.Title = string.IsNullOrEmpty(_problemDetails.Title) ? "An error occurred while processing your request" : _problemDetails.Title;
                _problemDetails.Status = _problemDetails.Status ?? StatusCodes.Status400BadRequest;
                return _problemDetails;
            }
        }

        public CustomApiException(ProblemDetails problemDetails)
            : base((string)problemDetails.Detail)
        {
            _problemDetails = problemDetails;
        }
    }
}
