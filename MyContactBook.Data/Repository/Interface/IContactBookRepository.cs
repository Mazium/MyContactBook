using MyContactBook.Data.DTO;
using MyContactBook.Model.Entities;
using MyContactBook.Utilities;

namespace MyContactBook.Data.Repository.Interface
{
    public interface IContactBookRepository
    {
        Task<Contact> GetContactByIdAsync(string userId, int contactId);
        Task<IEnumerable<Contact>> GetContactsAsync(string userId, PagingParameter pagingParameter);
        void DeleteContact(Contact contact);
        Task AddContactForUserAsync(string userId, Contact contact);
        Task<bool> SaveChangesAsync();
        Task<IEnumerable<Contact>> GetContactsBySearchAsync(string userId, string searchTerm, PagingParameter pagingParameter);
        Task<bool> AppUserExistsAsync(string userId);
    }   
}
