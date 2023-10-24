using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyContactBook.Data.Context;
using MyContactBook.Data.DTO;
using MyContactBook.Data.Repository.Interface;
using MyContactBook.Model.Entities;
using MyContactBook.Utilities;
using System.Security.Claims;

namespace MyContactBook.Data.Repository.Implementation
{
    public class ContactBookRepository : IContactBookRepository
    {
        private readonly ContactBookDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
       

        public ContactBookRepository(ContactBookDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
           
        }
        public void  DeleteContact(Contact contact)
        {
            _dbContext.Contacts.Remove(contact);
        }

        public async Task<Contact> GetContactByIdAsync(string userId, int contactId)
        {
            return await _dbContext.Contacts.Where(p => p.UserId == userId && p.Id == contactId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Contact>> GetContactsAsync(string userId, PagingParameter pagingParameter)
        {
            
            pagingParameter.PageSize = Math.Min(pagingParameter.PageSize, PagingParameter.MaxPageSize);

            return await _dbContext.Contacts
                .Where(p => p.UserId == userId)
                .Skip((pagingParameter.Page - 1) * pagingParameter.PageSize)
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

        public async Task AddContactForUserAsync(string userId, Contact contact)
        {
            var appUser = await GetUserAsync(userId, false);

            if (appUser != null)
            {
                appUser.Contacts ??= new List<Contact>();

                appUser.Contacts.Add(contact);
                
            }
            else
            {
                throw new InvalidOperationException("User not found.");
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _dbContext.SaveChangesAsync() >= 0);
        }

        public async Task<IEnumerable<Contact>> GetContactsBySearchAsync(string userId, string searchTerm, 
            PagingParameter pagingParameter)
        {
            return await _dbContext.Contacts
                .Where(p => p.UserId == userId && p.Name.Contains(searchTerm))
                .Skip((pagingParameter.Page - 1) * pagingParameter.PageSize)
                .Take(pagingParameter.PageSize)
                .ToListAsync();
        }

        public async Task<bool> AppUserExistsAsync(string userId)
        {
            return await _dbContext.Users.AnyAsync(c => c.Id == userId);
        }




    }
}
