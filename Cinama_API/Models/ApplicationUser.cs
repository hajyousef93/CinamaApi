using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinama_API.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Country { get; set; }
    }
}
