using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyContactBook.Core.Services.Interface;
using MyContactBook.Data.DTO;
using MyContactBook.Utilities;

namespace MyContactAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactBookController : ControllerBase
    {
        private readonly IContactBookService _contactBookService;
        public ContactBookController(IContactBookService contactBookService)
        {
            _contactBookService = contactBookService ?? throw new ArgumentNullException(nameof(contactBookService));
        }

        [HttpGet("{userId}/{contactId}")]        
        public async Task<ActionResult<ContactDto>> GetContactById(string userId, int contactId)
        {
            var response = await _contactBookService.GetContactByIdAsync(userId, contactId);

            return Ok(response);
        }

        [HttpGet("all-contacts/{userId}")]        
        public async Task<ActionResult<IEnumerable<ContactDto>>> GetContactsByUserId(string userId, [FromQuery] PagingParameter pagingParameter)
        {
            try
            {
                var contacts = await _contactBookService.GetContactsByUserIdAsync(userId, pagingParameter);
               
                return Ok(contacts);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
       

        [HttpPost("contacts/{userId}")]
        [Authorize]
        public async Task<ActionResult> CreateContact(string userId, CreateNewContactDto createNewContact)
        {
            var contactDto = await _contactBookService.CreateContactAsync(userId, createNewContact);

            if (contactDto == null)
            {
                return NotFound("User not found or contact creation failed.");
            }

            return CreatedAtAction(nameof(CreateContact), new { userId = userId, contactId = contactDto.Id }, contactDto);
        }

        [HttpDelete("{userId}/{contactId}")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> DeleteContactAsync(string userId, int contactId)
        {
            var deletedContact = await _contactBookService.DeleteContactAsync(userId, contactId);

            if (deletedContact == null)
            {
                return NotFound("Contact not found or deletion failed.");
            }

            return Ok(deletedContact);
        }


        [HttpGet("search/{userId}")]        
        public async Task<ActionResult<IEnumerable<ContactDto>>> SearchContacts(
            string userId,
           [FromQuery] string searchTerm = "",
            [FromQuery] PagingParameter pagingParameter = null)
        {
            var contacts = await _contactBookService.GetContactsBySearchAsync(userId, searchTerm, pagingParameter);

            if (contacts == null || !contacts.Any())
            {
                return NotFound("No contacts found matching the search term.");
            }

            return Ok(contacts);
        }

    }
}
