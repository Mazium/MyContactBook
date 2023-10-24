using MyContactBook.Data.DTO;
using MyContactBook.Model.Entities;
using MyContactBook.Utilities;

namespace MyContactBook.Core.Services.Interface
{
    public interface IUserService
    {
        Task<IEnumerable<AppUserDto>> GetAllUsersAsync(PagingParameter pagingParameters);
        Task<bool> AppUserExistsAsync(string userId);
        Task<bool> UpdateUserDetailsAsync(string userId, UpdateUserDetailsDto updateUserDetailsDto);
        Task<bool> UpdateUserPhotoAsync(string userId, UpdatePhotoDto updatePhotoDto);
        Task<bool> DeleteUserAsync(string userId);
        Task<IEnumerable<AppUserDto>> SearchUsersAsync(string searchQuery);
        Task<AppUserDto> GetUserByIdAsync(string userId);
    }
}
