
namespace DtoLayer.Dtos.UserDtos
{
    public class UserProfileDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ProfilePicture { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
    }
}
