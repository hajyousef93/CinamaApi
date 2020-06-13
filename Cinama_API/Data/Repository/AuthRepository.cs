using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Cinama_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cinama_API.Data.Repository
{
    public class AuthRepository : IAuthRepository
    {
        public Task<bool> EmailExists(string Email)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> Login(string Email, string password, bool rememberMe)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> Register(ApplicationUser user, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserExists(string Username)
        {
            throw new NotImplementedException();
        }
    }
}
