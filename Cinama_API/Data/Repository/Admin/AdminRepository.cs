using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinama_API.Models;
using Cinama_API.ModelViews.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cinama_API.Data.Repository.Admin
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDb _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public AdminRepository(ApplicationDb db,UserManager<ApplicationUser>userManager, RoleManager<ApplicationRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApplicationUser> AddUserAsync(AddUserModel model)
        {
            if (model == null)
            {
                return null;
            }
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                EmailConfirmed = model.EmailConfirmed,
                Country = model.Country,
                PhoneNumber = model.PhoneNumber,
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (await _roleManager.RoleExistsAsync("User"))
                {
                    if (!await _userManager.IsInRoleAsync(user, "User") && !await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                    }

                }
                return user;
            }
            return null;
            
        }

        public async Task<bool> DeleteUser(List<string> ids)
        {
            if (ids.Count < 1)
            {
                return false;
            }
            var i = 0;
            foreach (string id in ids)
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (user == null)
                    return false;
                _db.Users.Remove(user);
                i++;
            }
            if (i > 0)
            {
                await _db.SaveChangesAsync();
            }
            return true;
        }

        public async Task<ApplicationUser> EditUser(EditUserModel model)
        {
            if (model.Id == null)
            {
                return null;
            }
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (user == null)
            {
                return null;
            }
            if (model.Password != user.PasswordHash)
            {
                var result = await _userManager.RemovePasswordAsync(user);
                if (result.Succeeded)
                {
                    await _userManager.AddPasswordAsync(user, model.Password);
                }
            }
            user.UserName = model.Username;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Country = model.Country;
            user.EmailConfirmed = model.EmailConfirmed;
            _db.Users.Attach(user);
            _db.Entry(user).Property(x => x.Email).IsModified = true;
            _db.Entry(user).Property(x => x.UserName).IsModified = true;
            _db.Entry(user).Property(x => x.PhoneNumber).IsModified = true;
            _db.Entry(user).Property(x => x.EmailConfirmed).IsModified = true;
            _db.Entry(user).Property(x => x.Country).IsModified = true;
            await _db.SaveChangesAsync();
            return user;



        }

        public async Task<bool> EditUserRoleAsync(EditUserRoleModel model)
        {
            if(model.roleId==null || model.userId == null)
            {
                return false;
            }
            var user =await _db.Users.FirstOrDefaultAsync(x => x.Id == model.userId);
            var role = await _db.Roles.FirstOrDefaultAsync(x => x.Id == model.roleId);
            if (user == null || role==null)
            {
                return false;
            }
            var currentRoleId = await _db.UserRoles.Where(x => x.UserId == model.userId).Select(x => x.RoleId).FirstOrDefaultAsync();
            var currentRoleName = await _db.Roles.Where(x => x.Id == currentRoleId).Select(x => x.Name).FirstOrDefaultAsync();
            var newRoleName = await _db.Roles.Where(x => x.Id == model.roleId).Select(x => x.Name).FirstOrDefaultAsync();
            if (await _userManager.IsInRoleAsync(user, currentRoleName))
            {
                var x = await _userManager.RemoveFromRoleAsync(user, currentRoleName);
                if (x.Succeeded)
                {
                    var s =await _userManager.AddToRoleAsync(user, newRoleName);
                    if (s.Succeeded)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<IEnumerable<ApplicationRole>> GetRolesAsync()
        {
            return await _db.Roles.ToListAsync();
        }

        public async Task<ApplicationUser> GetUser(string id)
        {
            if (id==null)
            {
                return null;
            }
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user==null)
            {
                return null;
            }
            return user;
        }

        public async Task<IEnumerable<UserRoleModel>> GetUserRoleAsync()
        {
            var query = await (
                from userRole in _db.UserRoles
                join user in _db.Users
                on userRole.UserId equals user.Id
                join role in _db.Roles
                on userRole.RoleId equals role.Id
                select new
                {
                    userRole.RoleId,
                    role.Name,
                    userRole.UserId,
                    user.UserName
                }).ToListAsync();

            List<UserRoleModel> userRoleModels = new List<UserRoleModel>();
            if (query.Count > 0)
            {
               
                for (int i = 0; i < query.Count; i++)
                {
                    var model = new UserRoleModel
                    {
                        RoleId = query[i].RoleId,
                        UserId = query[i].UserId,
                        RoleName = query[i].Name,
                        UserName = query[i].UserName
                    };
                    userRoleModels.Add(model); 
                };
                
            }return userRoleModels;
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsers()
        {
            return await _db.Users.ToListAsync();
        }
    }
}
