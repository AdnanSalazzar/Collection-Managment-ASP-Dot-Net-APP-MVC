namespace CollectionManagement.Models
{
    public class AdminUserViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; } // List of roles
    }
}
