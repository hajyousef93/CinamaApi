using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cinama_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        //[HttpGet]
        //[Route("GetAllUser")]
        //public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllUser()
        //{
        //    return await _db.Users.ToListAsync();

        //}
    }
}