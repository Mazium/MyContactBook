using MyContactBook.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyContactBook.Data.Repository.Interface
{
    public interface IAuthRepository
    {
        Task<bool> CreateUserAsync(CreateUserDto createUserDto, List<string> roles);
    }
}
