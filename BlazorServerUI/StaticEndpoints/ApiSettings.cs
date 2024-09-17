namespace BlazorServerUI.StaticEndpoints
{
    public class ApiSettings
    {
        public string BaseUrl { get; set; }
        public Endpoints Endpoints { get; set; }
    }

    public class Endpoints
    {
        public string Login { get; set; }
        public string Register { get; set; }
        public string UploadProfilePicture { get; set; }
        public string Logout { get; set; }
        public string GetFollowedUsersPosts { get; set; }
        public string AddComment { get; set; }
        public string LikePost { get; set; }
        public string GetCommentsForFollowedUsersPosts { get; set; }
        public string GetUserProfile { get; set; }
        public string DeleteProfilePicture { get; set; }
        public string GetFollowingUsers { get; set; }
        public string GetFollowers { get; set; }
        public string GetUserProfileById { get; set; }
        public string UnfollowUser { get; set; }
        public string FollowUser { get; set; }
        public string CheckFollowing { get; set; }
        public string ApproveFollowRequest { get; set; }
        public string RejectFollowRequest { get; set; }
        public string GetNotificationCount { get; set; }
        public string GetUserFollowRequestNotifications { get; set; }
        public string FollowBack { get; set; }
        public string ApproveFollowBack { get; set; }
        public string RejectFollowBack { get; set; }
        public string GetOtherUsers { get; set; }
        public string CreatePost { get; set; }
        public string GetUserPosts { get; set; }
        public string GetUserPostsIfFollowed { get; set; }
        public string GetCommentsForUserPosts { get; set; }
        public string GetPostLikes { get; set; }
        public string DeletePost { get; set; }
        public string UpdatePost { get; set; }
        public string GetUserAllNotifications { get; set; }
        public string SendMessage { get; set; }
        public string GetUserMessageThreads { get; set; }
        public string GetUserMessages { get; set; }
        public string UpdateMessage { get; set; }
        public string GetUnreadMessageCount { get; set; }
        public string MarkAsRead { get; set; }
        public string DeleteMessage { get; set; }
        public string UpdateUserName { get; set; }
        public string DeleteComment { get; set; }
        public string EditComment { get; set; }
    


    }

}
