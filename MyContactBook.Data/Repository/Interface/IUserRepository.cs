using MyContactBook.Model.Entities;
using MyContactBook.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyContactBook.Data.Repository.Interface
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetAllUsersAsync();
        Task<IEnumerable<AppUser>> GetPagedUsersAsync(PagingParameter pagingParameter);
        //Task<IEnumerable<AppUser>> GetAllUsersAsync(string FirstName, string searchQuery);
        Task<AppUser> GetUserAsync(string userId, bool includeContacts);
        Task<bool> AppUserExistsAsync(string userId);
        Task<bool> DeleteUserAsync(string userId);
        Task<IEnumerable<AppUser>> SearchUsersAsync(string searchQuery);
        Task<AppUser> GetUserByIdAsync(string userId);
    }
}
