using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyContactBook.Core.Services.Interface;
using MyContactBook.Data.DTO;
using MyContactBook.Data.Repository.Interface;
using MyContactBook.Data.Response;
using MyContactBook.Model.Domains;
using MyContactBook.Model.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;

namespace MyContactBook.Core.Services.Implementation
{
    public class AuthService: IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;
        public AuthService(IAuthRepository authRepository, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration)
        {
            _authRepository = authRepository ?? throw new ArgumentNullException(nameof(authRepository));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public async Task<bool> CreateUserAsync(CreateUserDto createUserDto, List<string> roles)
        {

            if (string.IsNullOrWhiteSpace(createUserDto.FirstName) ||
                string.IsNullOrWhiteSpace(createUserDto.LastName) ||
                string.IsNullOrWhiteSpace(createUserDto.Email) ||
                string.IsNullOrWhiteSpace(createUserDto.Password) ||
                string.IsNullOrWhiteSpace(createUserDto.ConfirmPassword))
            {

                return false;
            }

            if (createUserDto.Password != createUserDto.ConfirmPassword)
            {

                return false;
            }

            return await _authRepository.CreateUserAsync(createUserDto, roles);
        }
        public async Task<string> GenerateJwtToken(AppUser user, string role)
        {
            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>
            {
                new Claim("sub", user.Id),
                new Claim("given_name", user.FirstName),
                new Claim("family_name", user.LastName),
            };

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler()
                .WriteToken(jwtSecurityToken);

            return tokenToReturn;
        }
        public async Task<Response<LoginResponseDto>> LoginAsync(AuthenticationRequestBody authenticationRequestBody)
        {
            var response = new Response<LoginResponseDto>();
            if (string.IsNullOrEmpty(authenticationRequestBody.Email) || string.IsNullOrEmpty(authenticationRequestBody.Password))
            {
                throw new ArgumentException("Email and password are required");
            }

            var user = await _userManager.FindByEmailAsync(authenticationRequestBody.Email);

            if (user == null)
            {
                throw new AuthenticationException("User not found");
            }

            var result = await _signInManager.PasswordSignInAsync(user, authenticationRequestBody.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                var token = await GenerateJwtToken(user, role);
                var results = new LoginResponseDto
                {
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = token,
                    Id = user.Id
                };

                return response.Success("User logged in succesfully", StatusCodes.Status200OK, results);
            }

            throw new AuthenticationException("Login failed");
        }


    }
}
