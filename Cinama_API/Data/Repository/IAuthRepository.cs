using Cinama_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinama_API.Data.Repository
{
    public interface IAuthRepository
    {
        Task<ApplicationUser> Login(string Email,string password,bool rememberMe);
        Task<ApplicationUser> Register(ApplicationUser user,string password);
        Task<bool> UserExists(string Username);
        Task<bool> EmailExists(string Email);
        
        


    }
}
