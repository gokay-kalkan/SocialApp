

namespace DtoLayer.Dtos.CommentDtos
{
    public class GetCommentsForUserDto
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }

        public string UserId { get; set; }


        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }

        public string UserName { get; set; }
    }
}
