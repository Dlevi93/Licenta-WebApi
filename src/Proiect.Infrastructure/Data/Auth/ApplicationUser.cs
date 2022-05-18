using Microsoft.AspNetCore.Identity;

namespace Proiect.Infrastructure.Data.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
