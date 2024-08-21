using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CollectionManagement.Models.Domain
{
    public class Role : IdentityRole<int>
    {
        //[Key]
        //public int RoleId { get; set; }

        //[Required]
        //[StringLength(256)]
        //public string RoleName { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }
}
