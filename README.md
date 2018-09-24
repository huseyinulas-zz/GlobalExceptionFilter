# GlobalExceptionFilter

Global exception filter for creating api responses based on RFC 7807 standards

For api error message RFC Standards please see link below.

https://tools.ietf.org/html/rfc7807

## Installation

### Requirements

`$ install-package GlobalExceptionFilter`

## Usage
Add options filter HttpGlobalExceptionFilter
```  
public void ConfigureServices(IServiceCollection services)
{
   services.AddMvc(options => { options.Filters.Add(typeof(HttpGlobalExceptionFilter)); })
            .AddControllersAsServices();
}
```

To return ProblemDetails type response based on RFC document throw CustomApiException that takes ProblemDetails object from its constructor.

```
 throw new CustomApiException(new ProblemDetails()
                                         {
                                             Title = "Order Error!",
                                             Detail = "Order cannot be found with id 3",
                                             Status = (int)HttpStatusCode.NotFound
                                         });
```

To return ProblemDetails with additional error messages CustomApiException can take  ValidationProblemDetails object from its constructor.

```
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

```
Also you can create your custom ProblemDetails class that inherited from ProblemDetails.

```
public class OrderProblemDetail : ProblemDetails
{
    public int OrderId { get; set; }
}
    
 ```
 Also unhandled exceptions are caught by exception filter in development environment and its exception message is placed developerMessage section.
 
 
