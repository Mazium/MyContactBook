using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MyContactBook.Model.Entities
{
    public class Contact: BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }
        public string UserId { get; set; }
       

    }
}
