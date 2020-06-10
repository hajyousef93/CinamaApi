using Cinama_API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinama_API.Data
{
    public class ApplicationDb:IdentityDbContext<ApplicationUser,ApplicationRole,string>
    {
        public ApplicationDb(DbContextOptions<ApplicationDb>options):base(options)
        {

        }
    }
}
