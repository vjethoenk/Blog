using System.ComponentModel.DataAnnotations;

namespace Blog.Areas.Admin.Models
{
    public class User
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Role {  get; set; }
        public string Image {  get; set; }  
    }
}
