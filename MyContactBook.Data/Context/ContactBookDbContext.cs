using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyContactBook.Model.Entities;

namespace MyContactBook.Data.Context
{
    public class ContactBookDbContext: IdentityDbContext<AppUser>
    {
       
        public DbSet<Contact> Contacts { get; set; }

        public ContactBookDbContext(DbContextOptions<ContactBookDbContext> options): base(options)            
        {
            
        }

    }
}
