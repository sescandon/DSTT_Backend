using System.ComponentModel.DataAnnotations;

namespace DSTT_Backend.Models.User
{
    public class UserDTO
    {
        [Required]
        [StringLength(25, ErrorMessage = "Username must be at most 25 characters long")]
        [MinLength(5, ErrorMessage = "Username must be at least 5 characters long")]
        public string Username { get; set; } = null!;
    }
}
