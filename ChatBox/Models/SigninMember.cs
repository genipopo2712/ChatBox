using System.ComponentModel.DataAnnotations;

namespace ChatBox.Models
{
    public class SigninMember
    {
        [Required(ErrorMessage = " Please enter your {0}")]
        [Display(Name = "user name or email")]
        public string Usr { get; set; }
        [Required(ErrorMessage = " Please enter your {0}")]
        [Display(Name = "password")]
        public string Pwd { get; set; }
        public bool Rem { get; set; }
    }
}
