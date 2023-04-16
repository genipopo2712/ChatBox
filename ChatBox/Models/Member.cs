using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Xml.Linq;

namespace ChatBox.Models
{
    public class Member
    {
        public string? UserId { get; set; }
        [Required(ErrorMessage = " Please enter your {0}")]
        [Display(Name = "user name")]
        public string Username { get; set; }
        [Required(ErrorMessage = " Please enter your {0}")]
        [Display(Name = "password")]
        public string? Password { get; set; }
        [Required(ErrorMessage = " Please enter your {0}")]
        [Display(Name = "email")]
        public string Email { get; set; }
        [Required(ErrorMessage = " Please enter your {0}")]
        [Display(Name = "fullname")]
        public string? Fullname { get; set; }
        public string? Avatar { get; set; }
        public DateTime? LastTimeActive { get; set; }
    }
}
