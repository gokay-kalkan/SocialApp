namespace BlazorServerUI.Data.CommentDtos
{
    public class CreateCommentDto
    {
        public int PostId { get; set; }

        public string Content { get; set; }
    }
}
