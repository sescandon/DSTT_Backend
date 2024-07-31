namespace DSTT_Backend.Models.Message
{
    public class MessageModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Content { get; set; } = null!;
    }
}
