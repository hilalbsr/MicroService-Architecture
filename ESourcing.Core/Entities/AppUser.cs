using Microsoft.AspNetCore.Identity;

namespace ESourcing.Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsSeller { get; set; } //satıcı
        public bool IsBuyer { get; set; } //alıcı
        public bool IsAdmin { get; set; }
    }
}
