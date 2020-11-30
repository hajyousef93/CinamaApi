using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cinama_API.ModelViews.Users
{
    public class EditUserRoleModel
    {
        [Required]
        public string userId { get; set; }
        [Required]
        public string roleId { get; set; }
    }
}
