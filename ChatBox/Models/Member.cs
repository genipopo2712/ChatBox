using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Xml.Linq;

namespace ChatBox.Models
{
    public class Member
    {
        public string? UserId { get; set; }

        public string Username { get; set; }

        public string? Password { get; set; }

        public string Email { get; set; }

        public string? Fullname { get; set; }
        public string? Avatar { get; set; }
        DateTime? LastTimeActive { get; set; }
    }
}
