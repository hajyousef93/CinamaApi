using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinama_API.Data.Repository.Admin;
using Cinama_API.Models;
using Cinama_API.ModelViews.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cinama_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _repo;

        public AdminController(IAdminRepository repo)
        {
            _repo = repo;
        }
        [HttpGet]
        [Route("GetAllUser")]
        public async Task<IEnumerable<ApplicationUser>> GetAllUser()
        {
            var users= await _repo.GetUsers();
            if (users==null)
            {
                return null;
            }
            return users;
        }

        [HttpGet]
        [Route("GetUser/{id}")]
        public async Task<ActionResult<ApplicationUser>> GetUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _repo.GetUser(id);
            if (user != null)
            {
                return user;
            }
            return BadRequest() ;
        }
        [HttpPut]
        [Route("EditUser")]
        public async Task<ActionResult<ApplicationUser>>EditUser(EditUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = await _repo.EditUser(model);
            if (user != null)
            {
                return user;
            }
            return BadRequest();
        }



        [HttpPost]
        [Route("AddUser")]
        public async Task<IActionResult>  AddUser(AddUserModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _repo.AddUserAsync(model);
                if(user!=null)
                {
                    return Ok();
                }

            }
            return BadRequest();
        }

        [HttpPost]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUser(List<string> ids)
        {
            if (ids.Count < 1)
            {
                return BadRequest();
            }
            var result = await _repo.DeleteUser(ids);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }
        
        [HttpGet]
        [Route("GetUserRole")]
        public async Task<IEnumerable<UserRoleModel>> GetUserRole()
        {
            var usersRole = await _repo.GetUserRoleAsync();
            if (usersRole == null)
            {
                return null;
            }
            return usersRole;
        }
        [HttpGet]
        [Route("GetAllRoles")]
        public async Task<IEnumerable<ApplicationRole>> GetAllRoles()
        {
            var roles = await _repo.GetRolesAsync();
            if (roles == null)
            {
                return null;
            }
            return roles;
        }
        [HttpPut]
        [Route("EditUserRole")]
        public async Task<ActionResult> EditUserRole(EditUserRoleModel model)
        {
            if (ModelState.IsValid)
            {
                var x = await _repo.EditUserRoleAsync(model);
                if (x)
                {
                    return Ok();
                }
            }
            return BadRequest();
        }
    }
}