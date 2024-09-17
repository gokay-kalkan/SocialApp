

using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Abstract;
using DataAccessLayer.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BusinessLayer.ServiceRegistirations
{
    public static class Registiration
    {
        public static void RegistrationServiceBusiness(this IServiceCollection services)
        {
            services.AddScoped<CommentService, CommentManager>();
            services.AddScoped<FollowerService, FollowerManager>();
            services.AddScoped<PostLikeService, PostLikeManager>();
            services.AddScoped<MessageService, MessageManager>();
            services.AddScoped<NotificationService, NotificationManager>();
            services.AddScoped<PostService, PostManager>();
            services.AddScoped<FollowRequestService, FollowRequestManager>();
            services.AddScoped<FollowRequestNotificationService, FollowRequestNotificationManager>();
            services.AddScoped<CommentLikeService, CommentLikeManager>();
            services.AddScoped<MessageNotificationService, MessageNotificationmanager>();

           


            services.AddScoped<ICommentRepository, EfCommentRepository>();
            services.AddScoped<IFollowerRepository, EfFollowerRepository>();
            services.AddScoped<IPostLikeRepository, EfPostLikeRepository>();
            services.AddScoped<IMessageRepository, EfMessageRepository>();
            services.AddScoped<INotificationRepository, EfNotificationRepository>();
            services.AddScoped<IPostRepository, EfPostRepository>();
            services.AddScoped<IFollowRequestRepository, EfFollowRequestRepository>();
            services.AddScoped<IFollowRequestNotificationRepository, EfFollowRequestNotificationRepository>();
            services.AddScoped<ICommentLikeRepository, EfCommentLikeRepository>();
            services.AddScoped<IMessageNotificationRepository, EfMessageNotificationRepository>();

        }
    }
}
