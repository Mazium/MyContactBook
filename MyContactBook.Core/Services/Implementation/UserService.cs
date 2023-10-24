using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Identity;
using MyContactBook.Core.Services.Interface;
using MyContactBook.Data.DTO;
using MyContactBook.Data.Repository.Interface;
using MyContactBook.Model.Entities;
using MyContactBook.Utilities;

namespace MyContactBook.Core.Services.Implementation
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly Cloudinary _cloudinary;

        public UserService(IUserRepository userRepository, UserManager<AppUser> userManager, Cloudinary cloudinary)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _cloudinary = cloudinary ?? throw new ArgumentNullException(nameof(cloudinary));
        }
        public async Task<bool> AppUserExistsAsync(string userId)
        {
            return await _userRepository.AppUserExistsAsync(userId);
        }
        public async Task<IEnumerable<AppUserDto>> GetAllUsersAsync(PagingParameter pagingParameters)
        {
            var userEntities = await _userRepository.GetPagedUsersAsync(pagingParameters);

            var results = new List<AppUserDto>();
            foreach (var userEntity in userEntities)
            {
                results.Add(new AppUserDto
                {
                    Id = userEntity.Id,
                    FirstName = userEntity.FirstName,
                    LastName = userEntity.LastName,
                    UserName = userEntity.UserName,
                    Password = userEntity.PasswordHash,
                    Address = userEntity.Address,
                    Email = userEntity.Email,
                    ImageUrl = userEntity.ImageUrl
                });
            }

            return results;
        }
        public async Task<bool> UpdateUserDetailsAsync(string userId, UpdateUserDetailsDto updateUserDetailsDto)
        {
            try
            {

                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {

                    return false;
                }


                user.FirstName = updateUserDetailsDto.FirstName;
                user.LastName = updateUserDetailsDto.LastName;
                user.UserName = updateUserDetailsDto.UserName;
                user.Email = updateUserDetailsDto.Email;
                user.Address = updateUserDetailsDto.Address;
                user.ImageUrl = updateUserDetailsDto.ImageUrl;


                var result = await _userManager.UpdateAsync(user);

                return result.Succeeded;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return false;
            }
        }
        public async Task<bool> UpdateUserPhotoAsync(string userId, UpdatePhotoDto updatePhotoDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return false;
                }


                if (!string.IsNullOrWhiteSpace(updatePhotoDto.ImageUrl))
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(updatePhotoDto.ImageUrl),
                        Transformation = new Transformation().Crop("limit").Width(300).Height(300)
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        user.ImageUrl = uploadResult.SecureUrl.AbsoluteUri;
                    }
                    else
                    {
                        Console.Error.WriteLine(uploadResult.Error.Message);
                        return false;
                    }
                }

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return true;
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.Error.WriteLine(error.Description);
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return false;
            }
        }
        public async Task<bool> DeleteUserAsync(string userId)
        {
            return await _userRepository.DeleteUserAsync(userId);
        }
        public async Task<IEnumerable<AppUserDto>> SearchUsersAsync(string searchQuery)
        {
            var userEntities = await _userRepository.SearchUsersAsync(searchQuery);

            var results = userEntities.Select(userEntity => new AppUserDto
            {
                Id = userEntity.Id,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                UserName = userEntity.UserName,
                Password = userEntity.PasswordHash,
                Address = userEntity.Address,
                Email = userEntity.Email,
                ImageUrl = userEntity.ImageUrl
            });

            return results;
        }
        public async Task<AppUserDto> GetUserByIdAsync(string userId)
        {
            var userEntity = await _userRepository.GetUserByIdAsync(userId);

            if (userEntity == null)
            {
                return null;
            }

            return new AppUserDto
            {
                Id = userEntity.Id,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                UserName = userEntity.UserName,
                Password = userEntity.PasswordHash,
                Address = userEntity.Address,
                Email = userEntity.Email,
                ImageUrl = userEntity.ImageUrl
            };
        }

    }
}
