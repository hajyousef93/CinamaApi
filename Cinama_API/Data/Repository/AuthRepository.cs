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
        private readonly ApplicationDb _db;
        private readonly UserManager<ApplicationUser> _manager;

        public AuthRepository(ApplicationDb db,UserManager<ApplicationUser>manager )
        {
            _db = db;
            _manager = manager;
        }

       

        public async Task<bool> EmailExists(string Email)
        {
            return await _db.Users.AnyAsync(x => x.Email == Email);
        }

      

        public Task<ApplicationUser> Login(string Email, string password, bool rememberMe)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> Register(ApplicationUser user, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UserExists(string Username)
        {
            return await _db.Users.AnyAsync(x => x.UserName == Username);

        }
    }
}
