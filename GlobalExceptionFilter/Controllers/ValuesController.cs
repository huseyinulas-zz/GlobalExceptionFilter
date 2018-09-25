using System;
using System.Net;

using Core;

using Microsoft.AspNetCore.Mvc;

namespace GlobalExceptionFilter.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        [HttpGet("v1")]
        public string GetV1()
        {
            throw new CustomApiException(new ProblemDetails()
                                         {
                                             Status = (int)HttpStatusCode.NotFound,
                                             Detail = "Order cannot be found with id 3"
                                         });
        }

        [HttpGet("v2")]
        public string GetV2()
        {
            throw new CustomApiException(new ProblemDetails());
        }

        [HttpGet("v3")]
        public string GetV3()
        {
            throw new CustomApiException(new ProblemDetails()
                                         {
                                             Title = "Order Error!",
                                             Detail = "Order cannot be found with id 3",
                                             Status = (int)HttpStatusCode.NotFound
                                         });
        }

        [HttpGet("v4")]
        public string GetV4()
        {
            throw new NullReferenceException();
        }

        [HttpGet("v5")]
        public string GetV5()
        {
            throw new CustomApiException(new OrderProblemDetail()
                                         {
                                             OrderId = 3,
                                             Detail = "Order cannot be found!",
                                             Status = (int)HttpStatusCode.NotFound,
                                             Title = "Order Error!"
                                         });
        }

        [HttpGet("v6")]
        public string GetV6()
        {
            var validationProblemDetails = new ValidationProblemDetails()
                                           {
                                               Detail = "Error while creating account. Please see errors for detail.",
                                               Status = (int)HttpStatusCode.BadRequest,
                                               Title = "Register Error"
                                           };

            validationProblemDetails.Errors.Add("Email", new string[]
                                                         {
                                                             "Email address length must be greater than 5 characters",
                                                             "Email address must be email format"
                                                         });

            validationProblemDetails.Errors.Add("Age", new string[]
                                                       {
                                                           "Age must be minimum 18"
                                                       });

            throw new CustomApiException(validationProblemDetails);
        }
    }

    public class OrderProblemDetail : ProblemDetails
    {
        public int OrderId { get; set; }
    }
}
