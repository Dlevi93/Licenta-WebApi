using System.Collections.Generic;

namespace Proiect.Web.ApiModels
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string Username { get; set; }
        public IList<string> Roles { get; set; }
        public UserDTO(string id, string email, string fullName, string userName, IList<string> roles)
        {
            Id = id;
            Email = email;
            Fullname = fullName;
            Username = userName;
            Roles = roles;
        }
    }
}
