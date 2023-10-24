using MyContactBook.Data.DTO;
using MyContactBook.Model.Entities;
using MyContactBook.Utilities;

namespace MyContactBook.Core.Services.Interface
{
    public interface IContactBookService
    {
        Task<ContactDto> GetContactByIdAsync(string userId, int contactId);
        Task<IEnumerable<ContactDto>> GetContactsByUserIdAsync(string userId, PagingParameter pagingParameter);
        Task<ContactDto> DeleteContactAsync(string userId,int contactId);
        Task<ContactDto> CreateContactAsync(string userId, CreateNewContactDto createNewContact);
        Task<IEnumerable<ContactDto>> GetContactsBySearchAsync(string userId, string searchTerm, PagingParameter pagingParameter);        
        Task<bool> SaveChangesAsync();
       
       
    }
}
