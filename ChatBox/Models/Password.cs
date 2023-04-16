using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ChatBox.Models
{
    public class Password
    {
        [Required(ErrorMessage = " Please enter your {0}")]
        [Display(Name = "old password")]
        public string Pwd { get; set; }
        [Required(ErrorMessage = " Please enter your {0}")]
        [Display(Name = "new password")]
        public string NewPwd { get; set; }
        
    }
}
