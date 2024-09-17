using Microsoft.AspNetCore.Identity;

namespace EntityLayer.Entities
{
    public enum Gender
    {
        Male = 1,
        Female = 2
    }
    public class User:IdentityUser
    {
       
        public string? ProfilePicture { get; set; }
        public DateTime CreatedDate { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }

        public DateTime? BirthDate { get; set; }  // Doğum tarihi
        public Gender Gender { get; set; }
        public virtual ICollection<Follower> Followers { get; set; }

        // Kullanıcının takip ettikleri
        public virtual ICollection<Follower> Following { get; set; }

        public virtual ICollection<FollowRequest> SentFollowRequests { get; set; }

        // Takip isteği alan kullanıcı için
        public virtual ICollection<FollowRequest> ReceivedFollowRequests { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Message> MessagesSent { get; set; }  // Gönderilen mesajlar
        public virtual ICollection<Message> MessagesReceived { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<PostLike> PostLikes { get; set; }
        public virtual ICollection<FollowRequestNotification> FollowRequestNotifications { get; set; }
        public virtual ICollection<MessageNotification> MessageNotifications { get; set; }

       
    }
}
