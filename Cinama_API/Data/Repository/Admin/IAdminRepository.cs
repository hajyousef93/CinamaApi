using Cinama_API.Models;
using Cinama_API.ModelViews.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinama_API.Data.Repository.Admin
{
    public interface IAdminRepository
    {
        Task<IEnumerable<ApplicationUser>> GetUsers();
        Task<ApplicationUser> AddUser(AddUserModel model);
        Task<ApplicationUser> GetUser(string id);
        Task<ApplicationUser> EditUser(EditUserModel model);
        Task<bool> DeleteUser(List<string> ids);
    }
}
