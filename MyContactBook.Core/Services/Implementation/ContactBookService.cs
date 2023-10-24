using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;
using MyContactBook.Core.Services.Interface;
using MyContactBook.Data.DTO;
using MyContactBook.Data.Repository.Interface;
using MyContactBook.Model.Entities;
using MyContactBook.Utilities;

namespace MyContactBook.Core.Services.Implementation
{
    public class ContactBookService : IContactBookService
    {
        private readonly IContactBookRepository _contactBookRepository;
        

        public ContactBookService(IContactBookRepository contactBookRepository)
        {
            _contactBookRepository = contactBookRepository ?? throw new ArgumentNullException(nameof(contactBookRepository));         
        }

        public async Task<ContactDto> DeleteContactAsync(string userId, int contactId)
        {
            if (!await _contactBookRepository.AppUserExistsAsync(userId))
            {
                return null;
            }

            var contactEntity = await _contactBookRepository.GetContactByIdAsync(userId, contactId);
            if (contactEntity == null)
            {
                return null;
            }

            _contactBookRepository.DeleteContact(contactEntity);
            await _contactBookRepository.SaveChangesAsync();

            // Return the deleted contact or a confirmation message.
            var deletedContactDto = new ContactDto
            {
                Id = contactEntity.Id,
                Name = contactEntity.Name,
                Email = contactEntity.Email,
                PhoneNumber = contactEntity.PhoneNumber,
                Address = contactEntity.Address
            };

            return deletedContactDto;
        }
  
        public async Task<ContactDto> GetContactByIdAsync(string userId, int contactId)
        {
            if (!await _contactBookRepository.AppUserExistsAsync(userId))
            {
                return null;
            }
            var contact = await _contactBookRepository.GetContactByIdAsync(userId, contactId);

            if (contact == null)
            {
                return null;
            }
            var result = new ContactDto
            {

                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                PhoneNumber = contact.PhoneNumber,
                Address = contact.Address,
                
            };

            return result;

        }

        public async Task<IEnumerable<ContactDto>> GetContactsByUserIdAsync(string userId, PagingParameter pagingParameter)
        {
            try
            {
                if (!await _contactBookRepository.AppUserExistsAsync(userId))
                {
                    throw new Exception($"User with ID {userId} not found.");
                }

                var contactEntities = await _contactBookRepository.GetContactsAsync(userId, pagingParameter);

                // Apply pagination to contactEntities
                var pagedContactEntities = contactEntities
                    .Skip((pagingParameter.Page - 1) * pagingParameter.PageSize)
                    .Take(pagingParameter.PageSize);

                var results = pagedContactEntities
                    .Select(contactEntity => new ContactDto
                    {
                        Id = contactEntity.Id,
                        Name = contactEntity.Name,
                        Email = contactEntity.Email,
                        PhoneNumber = contactEntity.PhoneNumber,
                        Address = contactEntity.Address,
                    });

                return results;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                
                throw;
            }
        }

        public async Task<ContactDto> CreateContactAsync(string userId, CreateNewContactDto createNewContact)
        {
            if (!await _contactBookRepository.AppUserExistsAsync(userId))
            {
                return null;
            }


            var newContact = new Contact()
            {
                Name = createNewContact.Name,
                Email = createNewContact.Email,
                PhoneNumber = createNewContact.PhoneNumber,
                Address = createNewContact.Address,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _contactBookRepository.AddContactForUserAsync(userId, newContact);
            await _contactBookRepository.SaveChangesAsync();

            var contactDto = new ContactDto
            {
                Id = newContact.Id,
                Name = newContact.Name,
                Email = newContact.Email,
                PhoneNumber = newContact.PhoneNumber,
                Address = newContact.Address
            };

            return contactDto;
        }
        public Task<bool> SaveChangesAsync()
        {
            return _contactBookRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ContactDto>> GetContactsBySearchAsync(string userId, 
            string searchTerm, PagingParameter pagingParameter)
        {
            var contacts = await _contactBookRepository.GetContactsBySearchAsync(userId, searchTerm, pagingParameter);

            if (contacts == null || !contacts.Any())
            {
                return Enumerable.Empty<ContactDto>();
            }

            var contactDtos = contacts.Select(contact => new ContactDto
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                PhoneNumber = contact.PhoneNumber,
                Address = contact.Address,
            });

            return contactDtos;
        }

       

    }
}
