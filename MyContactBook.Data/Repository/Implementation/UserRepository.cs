using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyContactBook.Data.Context;
using MyContactBook.Data.Repository.Interface;
using MyContactBook.Model.Entities;
using MyContactBook.Utilities;

namespace MyContactBook.Data.Repository.Implementation
{
    public class UserRepository: IUserRepository
    {
        private readonly ContactBookDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public UserRepository(ContactBookDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

        }
        public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
        {
            return await _dbContext.Users.OrderBy(c => c.LastName).ToListAsync();
        }
        public async Task<IEnumerable<AppUser>> GetPagedUsersAsync(PagingParameter pagingParameter)
        {
            pagingParameter.PageSize = Math.Min(pagingParameter.PageSize, PagingParameter.MaxPageSize);

            var skipAmount = (pagingParameter.Page - 1) * pagingParameter.PageSize;

            return await _dbContext.Users
                .OrderBy(c => c.LastName)
                .Skip(skipAmount)
                .Take(pagingParameter.PageSize)
                .ToListAsync();
        }
        public async Task<AppUser> GetUserAsync(string userId, bool includeContacts)
        {
            if (includeContacts)
            {
                return await _dbContext.Users.Include(c => c.Contacts)
                    .Where(c => c.Id == userId).FirstOrDefaultAsync();
            }

            return await _dbContext.Users
                .Where(c => c.Id == userId).FirstOrDefaultAsync();
        }
        public async Task<bool> AppUserExistsAsync(string userId)
        {
            return await _dbContext.Users.AnyAsync(c => c.Id == userId);
        }
        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }
            user.IsDeleted = true;


            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }
        public async Task<IEnumerable<AppUser>> SearchUsersAsync(string searchQuery)
        {
            return await _dbContext.Users
                .Where(user => user.UserName.Contains(searchQuery) || user.FirstName.Contains(searchQuery) || user.LastName.Contains(searchQuery))
                .ToListAsync();
        }
        public async Task<AppUser> GetUserByIdAsync(string userId)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == userId);
        }


    }
}
