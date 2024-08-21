using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CollectionManagement.Models.Domain
{
    public class User : IdentityUser<int>
    {
        //[Key]
        //public int UserId { get; set; }

        //[Required]
        //[StringLength(256)]
        //public string UserName { get; set; }

        //[Required]
        //[StringLength(256)]
        //public string Email { get; set; }

        //[Required]
        //public string PasswordHash { get; set; }

        public bool IsBlocked { get; set; }

        [StringLength(10)]
        public string SelectedLanguage { get; set; } = "en";

        [StringLength(10)]
        public string SelectedTheme { get; set; } = "light";

        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<Collection> Collections { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like> Likes { get; set; }
    }
}
