namespace BlazorServerUI.Data.CommentDtos
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }

        public string UserName { get; set; } // Yorumu yapan kullanıcı adı
    }
}
