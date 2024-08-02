using System.ComponentModel.DataAnnotations;

namespace DSTT_Backend.Models.Message
{
    public class MessagePutDTO
    {
        [Required]
        [StringLength(50, ErrorMessage = "Message must be at most 25 characters long")]
        [MinLength(5, ErrorMessage = "Message must be at least 5 characters long")]
        public string Content { get; set; } = null!;
    }
}
