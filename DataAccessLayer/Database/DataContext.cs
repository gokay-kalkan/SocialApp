using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace DataAccessLayer.Database
{
    public class DataContext : IdentityDbContext<User, IdentityRole, string>
    {
        public DataContext()
        {

        }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Follower> Followers { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<CommentLike> CommentLikes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<FollowRequest> FollowRequests { get; set; }
        public DbSet<FollowRequestNotification> FollowRequestNotifications { get; set; }
        public DbSet<MessageNotification> MessageNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Follower>()
         .HasOne(f => f.FollowerUser)
         .WithMany(u => u.Followers)
         .HasForeignKey(f => f.FollowerUserId)
         .OnDelete(DeleteBehavior.Restrict); // Cascade delete olmaması için Restrict kullanıyoruz

            // FollowingUser ile User arasındaki ilişki
            builder.Entity<Follower>()
                .HasOne(f => f.FollowingUser)
                .WithMany(u => u.Following)
                .HasForeignKey(f => f.FollowingUserId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Message>()
        .HasOne(m => m.Sender)
        .WithMany(u => u.MessagesSent)
        .HasForeignKey(m => m.SenderId)
        .OnDelete(DeleteBehavior.Restrict);

            // Message.Receiver ile User arasındaki ilişki
            builder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.MessagesReceived)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Comment>()
       .HasOne(c => c.Post)
       .WithMany(p => p.Comments)
       .HasForeignKey(c => c.PostId)
       .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PostLike>()
       .HasOne(c => c.Post)
       .WithMany(p => p.PostLikes)
       .HasForeignKey(c => c.PostId)
       .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CommentLike>()
      .HasOne(c => c.Comment)
      .WithMany(p => p.CommentLikes)
      .HasForeignKey(c => c.CommentId)
      .OnDelete(DeleteBehavior.Restrict);

            // FollowRequest ve User arasında Requester olarak ilişki
            builder.Entity<FollowRequest>()
                .HasOne(fr => fr.Requester)
                .WithMany(u => u.SentFollowRequests)
                .HasForeignKey(fr => fr.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            // FollowRequest ve User arasında TargetUser olarak ilişki
            builder.Entity<FollowRequest>()
                .HasOne(fr => fr.TargetUser)
                .WithMany(u => u.ReceivedFollowRequests)
                .HasForeignKey(fr => fr.TargetUserId)
                .OnDelete(DeleteBehavior.Restrict);




            base.OnModelCreating(builder);



        }
    }
}
