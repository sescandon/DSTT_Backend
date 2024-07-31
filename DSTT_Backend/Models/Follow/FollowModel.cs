namespace DSTT_Backend.Models.Follow
{
    public class FollowModel
    {
        public int Id { get; set; }

        public int FollowerId { get; set; }

        public int FollowedId { get; set; }
    }
}
