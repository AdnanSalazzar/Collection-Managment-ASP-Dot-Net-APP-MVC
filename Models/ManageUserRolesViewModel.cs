using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace CollectionManagement.Models
{
    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }  // Represents Admin role
        public bool IsUser { get; set; }   // Represents User role
        public bool IsBlocked { get; set; } // Indicates if the user is blocked
    }
}
