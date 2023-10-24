using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyContactBook.Data.DTO
{
    public class RegisterDto
    {       
            public CreateUserDto CreateUserDto { get; set; }
            public List<string> Roles { get; set; }
        

    }
}
