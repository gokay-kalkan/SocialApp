namespace EntityLayer.Entities
{
    public class Follower
    {
        public int FollowerId { get; set; }
        public string FollowerUserId { get; set; }
        public string FollowingUserId { get; set; }

        public virtual User FollowerUser { get; set; }  // Navigation Property
        public virtual User FollowingUser { get; set; }  // Navigation Property
    }
}
