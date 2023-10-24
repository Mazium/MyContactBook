using Microsoft.AspNetCore.Mvc;
using MyContactBook.Core.Services.Interface;
using MyContactBook.Data.DTO;
using MyContactBook.Model.Domains;



namespace MyContactAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

               
        public AuthController(IAuthService authService)            
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<string>> Login(AuthenticationRequestBody authenticationRequestBody)
        {
            var token = await _authService.LoginAsync(authenticationRequestBody);
            return Ok(token);
        }

        [HttpPost("register")]

        public async Task<ActionResult> RegisterUser([FromBody] RegisterDto registerUserDto)
        {

            var result = await _authService.CreateUserAsync(registerUserDto.CreateUserDto, registerUserDto.Roles);

            return Ok(result);

        }




    }
}
