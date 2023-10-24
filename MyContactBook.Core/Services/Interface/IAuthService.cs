using Microsoft.AspNetCore.Mvc;
using MyContactBook.Data.DTO;
using MyContactBook.Data.Response;
using MyContactBook.Model.Domains;
using MyContactBook.Model.Entities;

namespace MyContactBook.Core.Services.Interface
{
    public interface IAuthService
    {
        Task<bool> CreateUserAsync(CreateUserDto createUserDto, List<string> roles);
        Task<string> GenerateJwtToken(AppUser user, string role);
        public Task<Response<LoginResponseDto>> LoginAsync(AuthenticationRequestBody authenticationRequestBody);
    }
}
