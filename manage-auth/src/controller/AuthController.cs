using manage_auth.src.clients;
using manage_auth.src.models.requests;
using manage_auth.src.repository;
using manage_auth.src.util;
using Microsoft.AspNetCore.Mvc;

namespace manage_auth.src.controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        IRequestValidator _validator;
        IAuthRepository _authRepository;
        ICognitoClient _cognitoClient;

        public AuthController(IAuthRepository authRepository, IRequestValidator requestValidator, ICognitoClient cognitoClient)
        {
            _validator = requestValidator;
            _authRepository = authRepository;
            _cognitoClient = cognitoClient;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetBearerToken(int userId)
        {
            if (_validator.ValidateGetBearerToken(userId))
            {
                try
                {
                    return Ok();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    throw;
                }
            }
            else
            {
                return BadRequest("Task ID is required.");
            }
        }

        [HttpPost("authorize")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult AuthorizeRequest(AuthorizeRequest authorizeRequestRequest)
        {
            if (_validator.ValidateAuthorizeRequest(authorizeRequestRequest))
            {
                try
                {
                    return Ok();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    throw;
                }
            }
            else
            {
                return BadRequest("CreateUserRequest is required.");
            }
        }

        [HttpPost("user/authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AuthenticateUser(AuthenticateUserRequest authenticateUserRequest)
        {
            if (_validator.ValidateAuthenticateUser(authenticateUserRequest))
            {
                try
                {
                    var authResponse = await _cognitoClient.AuthenticateUserAsync(authenticateUserRequest);
                    return Ok(authResponse);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    throw;
                }
            }
            else
            {
                return BadRequest("CreateUserRequest is required.");
            }
        }

        [HttpPost("user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterUser(CreateUser createUserRequest)
        {
            if (_validator.ValidateCreateUser(createUserRequest))
            {
                try
                {
                    await _cognitoClient.SignUpUserAsync(createUserRequest.Email, createUserRequest.Password, createUserRequest.FirstName, createUserRequest.LastName);
                    return Ok();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    throw;
                }
            }
            else
            {
                return BadRequest("CreateUserRequest is required.");
            }
        }
        
        [HttpPost("user/confirm")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ConfirmUser(ConfirmUserRequest confirmUserRequest)
        {
            if (_validator.ValidateConfirmUser(confirmUserRequest))
            {
                try
                {
                    var confirmSignUpResponse = await _cognitoClient.ConfirmUserAsync(confirmUserRequest);
                    return Ok();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    throw;
                }
            }
            else
            {
                return BadRequest("CreateUserRequest is required.");
            }
        }
    }
}