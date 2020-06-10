using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Cinama_API.Data;
using Cinama_API.Data.Repository;
using Cinama_API.Models;
using Cinama_API.ModelViews;
using Cinama_API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinama_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDb _db;
        private readonly IAuthRepository _auth;
        private readonly UserManager<ApplicationUser> _manager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountController(ApplicationDb db,IAuthRepository auth, UserManager<ApplicationUser> manager, SignInManager<ApplicationUser> signInManager,RoleManager<ApplicationRole>roleManager)
        {
            _db = db;
            _auth = auth;
            _manager = manager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _auth.EmailExists(model.Email))
                {
                    return BadRequest("Email is used");
                }
                if (await _auth.UserExists(model.Username))
                {
                    return BadRequest("Username is used");
                }

                else
                {
                    var user = new ApplicationUser
                    {
                        UserName = model.Username,
                        PhoneNumber = model.Phone,
                        Country = model.Country,
                        Email = model.Email

                    };
                    var result = await _manager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        var token = await _manager.GenerateEmailConfirmationTokenAsync(user);
                        var ConfirmLink = Url.Action("ConfirmEmail", "Account", new
                        {
                            id = user.Id,
                            token = HttpUtility.UrlEncode(token)
                        }, Request.Scheme);
                        var txt = "Please confirm your registration at our site";
                        var link = "<a href=\"" + ConfirmLink + "\">Confirm registration</a>";
                        var subject = "Registeration  Confirm";
                        if (await SenderGridApi.Execute(user.Email, user.UserName, subject, txt, link))
                        {
                            return StatusCode(StatusCodes.Status200OK);
                        }
                    }
                    else
                    {
                        return BadRequest(result.Errors);
                    }
                }
            }
            return StatusCode(StatusCodes.Status400BadRequest);
        }


        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string id, string token)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(token))
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            var user = await _manager.FindByIdAsync(id);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            var result = await _manager.ConfirmEmailAsync(user, HttpUtility.UrlDecode(token));
            if (result.Succeeded)
            {
                return Ok("Registration Succeesed");
            }
            else
            {
                return BadRequest();
            }

        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            //Creat Role Admin && User 
            await CreateRole();
            //Create Admin User 
            await CreateAdmin();

            //Login
            if (model == null)
                return NotFound();
            var user = await _manager.FindByEmailAsync(model.Email);
            if (user == null)
                return NotFound();
            if (!user.EmailConfirmed)
            {
                return Unauthorized("Email is not Confirm yet!!");
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);
            if (result.Succeeded)
            {
                //set role name for user
                if (await _roleManager.RoleExistsAsync("User"))
                {
                    if (!await _manager.IsInRoleAsync(user,"User"))
                    {
                        await _manager.AddToRoleAsync(user, "User");
                    }
                    
                }
                var roleName = await GetRoleNameByUserId(user.Id);
                if (roleName != null)
                    AddCookies(user.UserName, user.Id, roleName, model.RememberMe);
               
                return StatusCode(StatusCodes.Status200OK);
            }
            else if (result.IsLockedOut)
            {
                return Unauthorized("User account is lockout");
            }
            return StatusCode(StatusCodes.Status204NoContent);
        }

        [HttpGet]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        private async Task<string> GetRoleNameByUserId(string userId)
        {
            var userRole = await _db.UserRoles.FirstOrDefaultAsync(x => x.UserId == userId);
            if (userRole!=null)
            {
                return await _db.Roles.Where(x => x.Id == userRole.RoleId).Select(x => x.Name).FirstOrDefaultAsync();
            }
            return null;
        }
        [Authorize]
        [HttpGet]
        [Route("GetAllUser")]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllUser()
        {
            return await _db.Users.ToListAsync();
            
        }
        private async Task CreateAdmin()
        {
            var admin = await _manager.FindByNameAsync("Admin");
            if (admin== null)
            {
                var user = new ApplicationUser
                {
                    UserName = "Admin",
                    Email = "admin@admin.com",
                    EmailConfirmed = true

                };
                var x = await _manager.CreateAsync(user, "123@dmin");
                if (x.Succeeded)
                {
                    if (await _roleManager.RoleExistsAsync("Admin"))
                    {
                        await _manager.AddToRoleAsync(user, "Admin");
                    }
                    
                }
            }
        }

        private async Task CreateRole()
        {
            if (_roleManager.Roles.Count()<1)
            {
                var role = new ApplicationRole
                {
                    Name = "Admin"
                };
                await _roleManager.CreateAsync(role);

                role = new ApplicationRole
                {
                    Name = "User"
                };
                await _roleManager.CreateAsync(role);
            }
        }


        /// <summary>
        /// Add Cookies
        /// </summary>
       
        public async void AddCookies(string username, string userId, string roleName, bool remember)
        {
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.Name,username),
                new Claim(ClaimTypes.NameIdentifier,userId),
                new Claim(ClaimTypes.Role,roleName),

            };

            var claimIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
            if (remember)
            {
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = remember,
                    ExpiresUtc = DateTime.UtcNow.AddDays(10)
                };

                await HttpContext.SignInAsync
               (
                   CookieAuthenticationDefaults.AuthenticationScheme,
                   new ClaimsPrincipal(claimIdentity),
                   authProperties
               );

            }
            else 
            {
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = remember,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                };
                await HttpContext.SignInAsync
              (
                  CookieAuthenticationDefaults.AuthenticationScheme,
                  new ClaimsPrincipal(claimIdentity),
                  authProperties
              );

            }
        }
    }
}