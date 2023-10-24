using Microsoft.AspNetCore.Identity;


namespace MyContactBook.Model.Entities
{
    public class AppUser: IdentityUser
    {        
        public string FirstName { get; set; }        
        public string LastName { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<Contact> Contacts { get; set; }
        public string Address { get; set; }
        public bool IsDeleted { get; set; }

    }
}
