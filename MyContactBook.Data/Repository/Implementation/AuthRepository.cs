using Microsoft.AspNetCore.Identity;
using MyContactBook.Data.Context;
using MyContactBook.Data.DTO;
using MyContactBook.Data.Repository.Interface;
using MyContactBook.Model.Entities;
using System.Security.Claims;

namespace MyContactBook.Data.Repository.Implementation
{
    public class AuthRepository: IAuthRepository
    {
        //private readonly ContactBookDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public AuthRepository(UserManager<AppUser> userManager)
        {
            //_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

        }
        public async Task<bool> CreateUserAsync(CreateUserDto createUserDto, List<string> roles)
        {
            try
            {
                var user = new AppUser
                {
                    UserName = createUserDto.Email,
                    Email = createUserDto.Email,
                    FirstName = createUserDto.FirstName,
                    LastName = createUserDto.LastName,
                    // Additional user properties
                };

                var result = await _userManager.CreateAsync(user, createUserDto.Password);

                if (result.Succeeded)
                {
                    // Add claims for additional user properties
                    await _userManager.AddClaimAsync(user, new Claim("FirstName", createUserDto.FirstName));
                    await _userManager.AddClaimAsync(user, new Claim("LastName", createUserDto.LastName));
                    await _userManager.AddClaimAsync(user, new Claim("Address", createUserDto.Address));
                    foreach (var roleName in roles)
                    {

                        await _userManager.AddToRoleAsync(user, roleName);
                    }

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
    }
}
