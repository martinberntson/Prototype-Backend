using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Prototype_Backend.Services;
using static Prototype_Backend.Records.Records;

namespace Prototype_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private UserService _service;
        public UserController(UserService userService)
        {
            _service = userService;
        }
        [HttpPut]
        public IActionResult Register(UserRegistrationQuery query)
        {
            if (query is null) return new BadRequestObjectResult("Bad Query");
            if (string.IsNullOrEmpty(query.FullName)) return new BadRequestObjectResult("First Name missing");
            if (string.IsNullOrEmpty(query.Username)) return new BadRequestObjectResult("Last Name missing");
            if (string.IsNullOrEmpty(query.Email)) return new BadRequestObjectResult("Email Name missing");
            if (string.IsNullOrEmpty(query.PhoneNumber)) return new BadRequestObjectResult("Phone Number Name missing");
            if (string.IsNullOrEmpty(query.Profile)) return new BadRequestObjectResult("Profile picture missing");
            if (string.IsNullOrEmpty(query.Password)) return new BadRequestObjectResult("Password missing");
            if (_service.CheckEmail(query.Email) is true) return new BadRequestObjectResult("Account with that email already exists");
            if (_service.CheckUsername(query.Username) is true) return new BadRequestObjectResult("Username already in use");

            UserRegistrationResponse response = _service.RegisterUser(query);
            return new OkObjectResult(response);
        }

        [HttpPost]
        public IActionResult Login(UserLoginRequest request)
        {
            if (request is null) return new BadRequestObjectResult("Empty Request");
            if (string.IsNullOrEmpty(request.Password)) return new BadRequestObjectResult("No password submitted");
            if (string.IsNullOrEmpty(request.Username)) return new BadRequestObjectResult("No username submitted");
            if (_service.CheckUsername(request.Username) is not true) return new BadRequestObjectResult("Username or password was incorrect");

            var result = _service.CheckPassword(request);
            if (string.IsNullOrEmpty(result)) return new BadRequestObjectResult("Something went wrong");
            if (result == "Not identical") return new BadRequestObjectResult("Username or password was incorrect");

            return new OkObjectResult(result);
        }
        [HttpPost("/Auth")]
        public IActionResult Auth(string jwt)
        {
            var result = _service.ValidateToken(jwt);
            if (result is null) return new BadRequestObjectResult("Failed to validate");
            return new OkObjectResult("Validated");
        }
    }
}
