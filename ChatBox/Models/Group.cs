using System.ComponentModel.DataAnnotations;

namespace ChatBox.Models
{
    public class Group
    {
        [Required(ErrorMessage = " Please add member to your group")]
        public string Members { get; set; }
        [Required(ErrorMessage = " Please enter your {0}")]
        [Display(Name = "group name")]
        public string Convname { get; set; }
        public string? ConvDescrip { get; set; }
        public string? Avatar { get; set; }
    }
}
