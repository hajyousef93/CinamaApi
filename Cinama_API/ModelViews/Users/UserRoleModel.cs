using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cinama_API.ModelViews.Users
{
    public class UserRoleModel
    {
        [Required]
        public string RoleId { get; set; }
        [Required]
        public string RoleName { get; set; }
        [Required]
        public string  UserId { get; set; }
        [Required]
        public string UserName { get; set; }
    }
}
